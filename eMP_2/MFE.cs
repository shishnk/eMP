namespace eMP_2;

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

        public MFEBuilder SetMethod(Decomposer solver) {
            _mfe._solver = solver;
            return this;
        }

        public static implicit operator MFE(MFEBuilder builder)
            => builder._mfe;
    }
    // default! указывает на то, что данное поле не может принимать null
    public delegate double Basis(double x);

    private readonly Integration _integration = new();
    private Basis[] _basis = default!;
    private Basis[] _dBasis = default!;
    private ITest _test = default!;
    private Decomposer _solver = default!;
    private IGrid _spaceGrid = default!;
    private IGrid _timeGrid = default!;
    private Matrix? _massMatrix; // матрица масс
    private Matrix _stiffnessMatrix = default!; // матрица жесткости
    // private DiagMatrix _globalMatrix = default!; // 
    private BandMatrix _remasteredGlobalMatrix = default!;
    private FiniteElement[] _elements = default!;
    private double[] _localVector = default!;
    private double[] _vector = default!; // вектор правой части

    public void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");
            ArgumentNullException.ThrowIfNull(_solver, $"{nameof(_solver)} cannot be null, set the method of solving SLAE");

            Init();
            // AssemblyGlobalMatrix();
            AssemblyRemasteredGlobalMatrix();
            AssemblyGlobalVector();
            AccountingDiriclhetBoundary();
            _solver.SetMatrix(_remasteredGlobalMatrix);
            _solver.SetVector(_vector);
            _solver.Compute();

            foreach (var weight in _solver.Solution!)
            Console.WriteLine(weight);

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void Init() {
        _massMatrix = (_spaceGrid.Sigma.HasValue && _spaceGrid.Sigma != 0) ? new(3) : null;
        _stiffnessMatrix = new(3);
        // _globalMatrix = new(2 * _spaceGrid.Points.Length - 1);
        _remasteredGlobalMatrix = new(2 * _spaceGrid.Points.Length - 1, 2);
        _vector = new double[_remasteredGlobalMatrix.Size];
        _localVector = new double[3];
        _basis = new Basis[] { QuadraticBasis.Psi1, QuadraticBasis.Psi2, QuadraticBasis.Psi3 };
        _dBasis = new Basis[] { QuadraticBasis.DPsi1, QuadraticBasis.DPsi2, QuadraticBasis.DPsi3 };
        _elements = new FiniteElement[_spaceGrid.Points.Length - 1];

        for (int ielem = 0, index = 0; ielem < _elements.Length; ielem++, index++) // формирование конечных элементов
            _elements[ielem] = new(new(_spaceGrid.Points[index], _spaceGrid.Points[index + 1]));
    }

    private void AssemblyLocalMatrices(int ielem) {
        for (int i = 0; i < _stiffnessMatrix.Size; i++) {
            for (int j = 0; j < _stiffnessMatrix.Size; j++) {
                _stiffnessMatrix[i, j] = _integration.GaussOrder5(_dBasis[i], _dBasis[j], 0, 1) / _elements[ielem].Interval.Lenght;

                if (_massMatrix is not null)
                    _massMatrix[i, j] = _integration.GaussOrder5(_basis[i], _basis[j], 0, 1) * _elements[ielem].Interval.Lenght;
            }
        }

        if (_massMatrix is not null)
            _stiffnessMatrix += _massMatrix;
    }

    // private void AssemblyGlobalMatrix() {
    //     for (int ielem = 0; ielem < _elements.Length; ielem++) {
    //         AssemblyLocalMatrices(ielem);
    //         // |  index   index    index    |
    //         // |  index   index+1  index+1  |
    //         // |  index   index+1  index+2  |

    //         int index = ielem * 2;

    //         _globalMatrix.Diags[0][index] += _stiffnessMatrix[0, 0];
    //         _globalMatrix.Diags[0][index + 1] += _stiffnessMatrix[1, 1];
    //         _globalMatrix.Diags[0][index + 2] += _stiffnessMatrix[2, 2];

    //         _globalMatrix.Diags[1][index] += _stiffnessMatrix[1, 0];
    //         _globalMatrix.Diags[1][index + 1] += _stiffnessMatrix[2, 1];
    //         _globalMatrix.Diags[2][index] += _stiffnessMatrix[2, 0];

    //         _globalMatrix.Diags[3][index] += _stiffnessMatrix[0, 1];
    //         _globalMatrix.Diags[3][index + 1] += _stiffnessMatrix[1, 2];
    //         _globalMatrix.Diags[4][index] += _stiffnessMatrix[0, 2];

    //         _stiffnessMatrix.Clear();
    //         _massMatrix?.Clear();
    //     }
    // }

    private void AssemblyRemasteredGlobalMatrix() {
        for (int ielem = 0; ielem < _elements.Length; ielem++) {
            AssemblyLocalMatrices(ielem);

            int index = 2 * ielem;

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

            _stiffnessMatrix.Clear();
            _massMatrix?.Clear();
        }
    }

    private void AssemblyGlobalVector() {
        for (int ielem = 0; ielem < _elements.Length; ielem++) {
            AssemblyLocalVector(ielem);

            int index = ielem * 2; // [ index, index+1, index+2 ]^T

            _vector[index] += _localVector[0];
            _vector[index + 1] += _localVector[1];
            _vector[index + 2] += _localVector[2];

            Array.Clear(_localVector);
        }
    }

    private void AssemblyLocalVector(int ielem) {
        double[] elementPoints = new double[3];

        elementPoints[0] = _elements[ielem].Interval.LeftBorder;
        elementPoints[1] = _elements[ielem].Interval.Center;
        elementPoints[2] = _elements[ielem].Interval.RightBorder;

        for (int i = 0; i < _localVector.Length; i++)
            _localVector[i] = _elements[ielem].Interval.Lenght * _test.F(elementPoints[i]) * _integration.GaussOrder5(_basis[i], 0, 1);

        /* заполнение локального вектора возможно через матрицу масс(разложения компонентов на линейный интерполянт), 
        но если в программе задать коэффициент перед ней нуль, то придется инициализировать ручками компоненты матрицы масс
         for (int i = 0; i < _massMatrix?.Size; i++)
             for (int j = 0; j < _massMatrix?.Size; j++)
                 _localVector[i] += _elements[ielem].Interval.Lenght * _test.F(elementPoints[j]) * _massMatrix[i, j]; */
    }

    private void AccountingDiriclhetBoundary() {
        double value = 1E+10;

        _remasteredGlobalMatrix.Diagonal[0] = _remasteredGlobalMatrix.Diagonal[^1] = value;
        _vector[0] = value * _test.U(_spaceGrid.Points[0]);
        _vector[^1] = value * _test.U(_spaceGrid.Points[^1]);
    }

    public static MFEBuilder CreateBuilder()
        => new();
}