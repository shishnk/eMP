namespace eMP_2;

public enum NonLinearSolverTypes {
    SimpleIteration,
    Newton
}

public class MFE {
    public class MFEBuilder {
        private readonly MFE _mfe;

        public MFEBuilder()
            => _mfe = new();

        public MFEBuilder SetTest(ITest test) {
            _mfe._test = test;
            return this;
        }

        public MFEBuilder SetSpaceGrid(IGrid grid) {
            _mfe._spaceGrid = grid;
            return this;
        }

        public MFEBuilder SetTimeGrid(IGrid grid) {
            _mfe._timeGrid = grid;
            return this;
        }

        public MFEBuilder SetDecomposer(Decomposer solver) {
            _mfe._solver = solver;
            return this;
        }

        public MFEBuilder SetMethod((NonLinearSolverTypes, int, double) method) {
            _mfe._method = method;
            return this;
        }

        public static implicit operator MFE(MFEBuilder builder)
            => builder._mfe;
    }
    // default! указывает на то, что данное поле не может принимать null
    public delegate double Basis(double x);

    private (NonLinearSolverTypes, int, double) _method;
    private readonly Integration _integration = new();
    private Basis[] _basis = default!;
    private Basis[] _dBasis = default!;
    private ITest _test = default!;
    private Decomposer _solver = default!;
    private IGrid _spaceGrid = default!;
    private IGrid _timeGrid = default!;
    private Matrix _massMatrix = default!; // матрица масс
    private Matrix _stiffnessMatrix = default!; // матрица жесткости
    private BandMatrix _remasteredGlobalMatrix = default!;
    private FiniteElement[] _elements = default!;
    private double[] _localVector = default!;
    private double[] _vector = default!; // вектор правой части
    private double[][] _layers = default!;

    public void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");
            ArgumentNullException.ThrowIfNull(_solver, $"{nameof(_solver)} cannot be null, set the method of solving SLAE");
            ArgumentNullException.ThrowIfNull(_method, $"{nameof(_method)} cannot be null, set the method of solving non-linear problem");

            Init();
            if (_method.Item1 == NonLinearSolverTypes.SimpleIteration)
                SimpleIteration();

            // MethodNewton();

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void Init() {
        _massMatrix = new(3);
        _stiffnessMatrix = new(3);
        _remasteredGlobalMatrix = new(_spaceGrid.Points.Length, 2);
        _vector = new double[_remasteredGlobalMatrix.Size];
        _localVector = new double[3];
        _basis = new Basis[] { QuadraticBasis.Psi1, QuadraticBasis.Psi2, QuadraticBasis.Psi3 };
        _dBasis = new Basis[] { QuadraticBasis.DPsi1, QuadraticBasis.DPsi2, QuadraticBasis.DPsi3 };
        _elements = new FiniteElement[(_spaceGrid.Points.Length - 1) / 2];
        _layers = new double[2].Select(item => new double[_remasteredGlobalMatrix.Size]).ToArray();

        // формирование конечных элементов
        for (int ielem = 0, index = 0; ielem < _elements.Length; ielem++, index += 2)
            _elements[ielem] = new(new(_spaceGrid.Points[index], _spaceGrid.Points[index + 2]));

        // начальное условие параболической задачи
        for (int i = 0; i < _spaceGrid.Points.Length; i++)
            _layers[0][i] = _test.U(_spaceGrid.Points[i], _timeGrid.Points[0]);
    }

    private void SimpleIteration() {
        int iters = 0;
        double residual = 0.0;

        _layers[0].Copy(_layers[1]);

        for (int itime = 1; itime < _timeGrid.Points.Length; itime++) {
            double timeDifference = _timeGrid.Points[itime] - _timeGrid.Points[itime - 1];

            AssemblySLAE(itime, timeDifference);
            AccountingDirichletBoundary(itime);

            for (iters = 0; iters < _method.Item2; iters++) {
                _solver.SetMatrix(_remasteredGlobalMatrix);
                _solver.SetVector(_vector);
                _solver.Compute();

                _solver.Solution!.Value.CopyTo(0, _layers[1], 0, _layers[1].Length);

                _remasteredGlobalMatrix.Clear();
                _vector.Fill(0);

                AssemblySLAE(itime, timeDifference);
                AccountingDirichletBoundary(itime);

                if ((residual = (_remasteredGlobalMatrix * _layers[1]).Sub(_vector).Norm() / _vector.Norm()) < _method.Item3)
                    break;
            }

            _layers[1].Copy(_layers[0]);
        }

        Console.WriteLine($"Iterations: {iters}");
        Console.WriteLine($"Residual:{residual}");

        foreach (var weight in _layers[1])
            Console.WriteLine(weight);

        // for report
        var exactValues = new double[_spaceGrid.Points.Length];

        for (int i = 0; i < exactValues.Length; i++)
            exactValues[i] = _test.U(_spaceGrid.Points[i], _timeGrid.Points[^1]);

        var sw = new StreamWriter("csv/test5.csv");
        using (sw) {
            for (int i = 0; i < _spaceGrid.Points.Length; i++) {
                if (i == 0) {
                    sw.WriteLine("$x_i$,Точное,Численное,Погрешность,Невязка,Кол-во итераций");
                    sw.WriteLine($"{_spaceGrid.Points[i]},{exactValues[i]},{_layers[1][i]},{exactValues.Sub(_layers[1]).Norm() / exactValues.Norm()},{residual:0.00E+0},{iters}");
                    continue;
                }

                sw.WriteLine($"{_spaceGrid.Points[i]},{exactValues[i]},{_layers[1][i]},,,");
            }
        }
    }

    private void AssemblyLocalMatrices(int ielem, double timeDifference) {
        double lambda(double point) => _layers[1][2 * ielem] * _basis[0](point) +
                                       _layers[1][2 * ielem + 1] * _basis[1](point) +
                                       _layers[1][2 * ielem + 2] * _basis[2](point);

        for (int i = 0; i < _stiffnessMatrix.Size; i++) {
            for (int j = 0; j < _stiffnessMatrix.Size; j++) {
                _stiffnessMatrix[i, j] = _integration.GaussOrder5(_test.Lambda(lambda), _dBasis[i], _dBasis[j], 0, 1) /
                                         _elements[ielem].Interval.Lenght;

                _massMatrix[i, j] = _integration.GaussOrder5(_basis[i], _basis[j], 0, 1) *
                                    _elements[ielem].Interval.Lenght * (_spaceGrid.Sigma is null || _spaceGrid.Sigma == 0 ? 1
                                    : _spaceGrid.Sigma.Value / timeDifference);
            }
        }

        if (_spaceGrid.Sigma is not null && _spaceGrid.Sigma != 0)
            _stiffnessMatrix += _massMatrix;
    }

    private void AssemblySLAE(int itime, double timeDifference) {
        _vector.Fill(0);
        _remasteredGlobalMatrix.Clear();
        
        for (int ielem = 0; ielem < _elements.Length; ielem++) {
            AssemblyLocalMatrices(ielem, timeDifference);
            AssemblyLocalVector(ielem, itime, timeDifference);

            int index = ielem * 2;

            _vector[index] += _localVector[0];
            _vector[index + 1] += _localVector[1];
            _vector[index + 2] += _localVector[2];

            _localVector.Fill(0);

            for (int i = 0; i < _stiffnessMatrix.Size; i++)
                _remasteredGlobalMatrix.Diagonal[index + i] += _stiffnessMatrix[i, i];

            // строчно-столбцовое хранение ленточной матрицы
            for (int i = 0; i < _stiffnessMatrix.Size - 1; i++) {
                for (int j = i + 1; j < _stiffnessMatrix.Size; j++) {
                    int tmpi = i;
                    int tmpj = j;

                    if (j > 1)
                        (tmpi, tmpj) = (j - 1, i);

                    _remasteredGlobalMatrix.Lower[index + tmpi + 1][tmpj] += _stiffnessMatrix[j, i];
                    _remasteredGlobalMatrix.Upper[index + tmpi + 1][tmpj] += _stiffnessMatrix[i, j]; // убрать для строчного хранения
                }
            }

            // строчное хранение
            // for (int i = 0; i < _stiffnessMatrix.Size - 1; i++) {
            //     for (int j = i + 1; j < _stiffnessMatrix.Size; j++) {
            //         int tmpj = j;

            //         if (i >= 1)
            //             tmpj--;

            //         _remasteredGlobalMatrix.Upper[index + i][tmpj - 1] += _stiffnessMatrix[i, j];
            //     }
            // }
        }
    }

    private void AssemblyLocalVector(int ielem, int itime, double timeDifference) {
        for (int i = 0; i < _massMatrix.Size; i++)
            for (int j = 0; j < _massMatrix.Size; j++)
                if (_spaceGrid.Sigma is null || _spaceGrid.Sigma == 0)
                    _localVector[i] += _test.F(_spaceGrid.Points[2 * ielem + j], _timeGrid.Points[itime]) * _massMatrix[i, j];
                else
                    _localVector[i] += _test.F(_spaceGrid.Points[2 * ielem + j], _timeGrid.Points[itime]) * _massMatrix[i, j] * timeDifference / _spaceGrid.Sigma.Value +
                                       _layers[0][2 * ielem + j] * _massMatrix[i, j];
    }

    private void AccountingDirichletBoundary(int itime) {
        _remasteredGlobalMatrix.Diagonal[0] = 1;
        _remasteredGlobalMatrix.Diagonal[^1] = 1;

        _remasteredGlobalMatrix.Upper[1][1] = 0;
        _remasteredGlobalMatrix.Upper[2][0] = 0;

        _remasteredGlobalMatrix.Lower[^1][0] = 0;
        _remasteredGlobalMatrix.Lower[^1][1] = 0;

        _vector[0] = _test.U(_spaceGrid.Points[0], _timeGrid.Points[itime]);
        _vector[^1] = _test.U(_spaceGrid.Points[^1], _timeGrid.Points[itime]);
    }

    public static MFEBuilder CreateBuilder()
        => new();
}