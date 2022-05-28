namespace eMP_3;

public class FEM {
    public class FEMBuilder {
        private readonly FEM _fem = new();

        public FEMBuilder SetTest(ITest test) {
            _fem._test = test;
            return this;
        }

        public FEMBuilder SetSpaceGrid(Grid grid) {
            _fem._grid = grid;
            return this;
        }

        public FEMBuilder SetSolverSLAE(Solver solver) {
            _fem._solver = solver;
            return this;
        }

        public FEMBuilder SetDecomposer(Decomposer decomposer) {
            _fem._decomposer = decomposer;
            return this;
        }

        public static implicit operator FEM(FEMBuilder builder)
            => builder._fem;
    }

    // default! указывает на то, что данное поле не может принимать null
    private delegate double Basis(Point3D point);
    private Basis[] _basis = default!;
    private ITest _test = default!;
    private Grid _grid = default!;
    private Solver? _solver;
    private Decomposer? _decomposer;
    private Matrix _massMatrix = default!; // матрица масс
    private Matrix _stiffnessMatrix = default!; // матрица жесткости
    private SparseMatrix _globalMatrix = default!;
    private Vector<double> _localVector1 = default!;
    private Vector<double> _localVector2 = default!;
    private Vector<double> _vector = default!; // вектор правой части

    public void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");

            if (_solver is null && _decomposer is null) {
                throw new ArgumentNullException(nameof(_solver), "Set the method of solving SLAE");
            }

            Init();
            Solve();
        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void Init() {
        _massMatrix = new(8);
        _stiffnessMatrix = new(8);
        _localVector1 = new(8);
        _localVector2 = new(8);

        _basis = new Basis[] { LinearBasis.Psi1, LinearBasis.Psi2, LinearBasis.Psi3,
                               LinearBasis.Psi4, LinearBasis.Psi5, LinearBasis.Psi6,
                               LinearBasis.Psi7, LinearBasis.Psi8};
    }

    private void InitSLAE(int sizeOffDiag) {
        _globalMatrix = new(2 * _grid.Points.Length, sizeOffDiag); // resizing in method ConstructPortrait()
        _vector = new(2 * _grid.Points.Length);
    }

    private void Solve() {
        ConstructPortrait();
        AssemblySLAE();

        AccountingDirichletBoundary();
        _globalMatrix.PrintDense("matrix.txt");

        if (_decomposer is not null) {
            _globalMatrix.AsProfileMatrix();
            _decomposer.SetMatrix(_globalMatrix);
            _decomposer.SetVector(_vector);
            _decomposer.Compute();
            ErrForward();
        } else {
            _solver!.SetMatrix(_globalMatrix);
            _solver.SetVector(_vector);
            _solver.Compute();
            ErrIter();
        }
    }

    private void ConstructPortrait() {
        List<int>[] list = new List<int>[_grid.Points.Length].Select(_ => new List<int>()).ToArray();

        for (int ielem = 0; ielem < _grid.Elements.Length; ielem++) {
            for (int i = 0; i < _grid.Elements[ielem].Length; i++) {
                for (int j = i + 1; j < _grid.Elements[ielem].Length; j++) {
                    int pos = _grid.Elements[ielem][j];
                    int elem = _grid.Elements[ielem][i];

                    if (!list[pos].Contains(elem)) {
                        list[pos].Add(elem);
                    }
                }
            }
        }

        list = list.Select(list => list.OrderBy(value => value).ToList()).ToArray();
        int sizeOffDiag = list.Sum(childList => childList.Count);

        InitSLAE(sizeOffDiag);

        _globalMatrix.ig[2] = 1;

        for (int i = 1; i < list.Length; i++) {
            _globalMatrix.ig[(2 * i) + 1] = _globalMatrix.ig[2 * i] + (2 * list[i].Count);
            _globalMatrix.ig[(2 * i) + 2] = _globalMatrix.ig[(2 * i) + 1] + (2 * list[i].Count) + 1;
        }

        _globalMatrix.jg = new int[_globalMatrix.ig[^1]];
        _globalMatrix.ggl = new double[_globalMatrix.ig[^1]];
        _globalMatrix.ggu = new double[_globalMatrix.ig[^1]];

        int index = 1;

        for (int i = 1; i < list.Length; i++) {
            for (int j = 0; j < list[i].Count; j++) {
                _globalMatrix.jg[index] = 2 * list[i][j];
                _globalMatrix.jg[index + 1] = (2 * list[i][j]) + 1;
                index += 2;
            }

            for (int k = 0; k < list[i].Count; k++) {
                _globalMatrix.jg[index] = 2 * list[i][k];
                _globalMatrix.jg[index + 1] = (2 * list[i][k]) + 1;
                index += 2;
            }

            _globalMatrix.jg[index] = 2 * i;
            index++;
        }
    }

    private void AssemblySLAE() {
        for (int ielem = 0; ielem < _grid.Elements.Length; ielem++) {
            AssemblyLocalMatrices(ielem);
            AssemblyLocalVector(ielem);

            for (int i = 0; i < _grid.Elements[ielem].Length; i++) {
                for (int j = 0; j < _grid.Elements[ielem].Length; j++) {
                    AddElementToGlobalMatrix(2 * _grid.Elements[ielem][i], 2 * _grid.Elements[ielem][j], _stiffnessMatrix[i, j]);
                    AddElementToGlobalMatrix((2 * _grid.Elements[ielem][i]) + 1, (2 * _grid.Elements[ielem][j]) + 1, _stiffnessMatrix[i, j]);
                    AddElementToGlobalMatrix(2 * _grid.Elements[ielem][i], (2 * _grid.Elements[ielem][j]) + 1, -_massMatrix[i, j]);
                    AddElementToGlobalMatrix((2 * _grid.Elements[ielem][i]) + 1, 2 * _grid.Elements[ielem][j], _massMatrix[i, j]);
                }
            }

            for (int i = 0; i < _localVector1.Length; i++) {
                _vector[2 * _grid.Elements[ielem][i]] += _localVector1[i];
                _vector[(2 * _grid.Elements[ielem][i]) + 1] += _localVector2[i];
            }

            _localVector1.Fill(0);
            _localVector2.Fill(0);
        }
    }

    private void AddElementToGlobalMatrix(int i, int j, double value) {
        if (i == j) {
            _globalMatrix.di[i] += value;
            return;
        }

        if (i < j) {
            for (int index = _globalMatrix.ig[j]; index < _globalMatrix.ig[j + 1]; index++) {
                if (_globalMatrix.jg[index] == i) {
                    _globalMatrix.ggu[index] += value;
                    return;
                }
            }
        } else {
            for (int index = _globalMatrix.ig[i]; index < _globalMatrix.ig[i + 1]; index++) {
                if (_globalMatrix.jg[index] == j) {
                    _globalMatrix.ggl[index] += value;
                    return;
                }
            }
        }
    }

    private void AssemblyLocalMatrices(int ielem) {
        double hx = Math.Abs(_grid.Points[_grid.Elements[ielem][1]].X - _grid.Points[_grid.Elements[ielem][0]].X);
        double hy = Math.Abs(_grid.Points[_grid.Elements[ielem][2]].Y - _grid.Points[_grid.Elements[ielem][0]].Y);
        double hz = Math.Abs(_grid.Points[_grid.Elements[ielem][4]].Z - _grid.Points[_grid.Elements[ielem][0]].Z);

        for (int i = 0; i < _stiffnessMatrix.Size; i++) {
            for (int j = 0; j < _stiffnessMatrix.Size; j++) {
                _stiffnessMatrix[i, j] = (_grid.Lambda * (Integration.GaussSegmentGrad(_basis[i].Invoke, _basis[j].Invoke, new(0, 0, 0), new(1, 1, 1)) /
                (hx * hy * hz))) - (_grid.Omega * _grid.Omega * _grid.Chi * (Integration.GaussSegment(_basis[i].Invoke, _basis[j].Invoke, new(0, 0, 0), new(1, 1, 1)) * (hx * hy * hz)));
            }
        }

        for (int i = 0; i < _massMatrix.Size; i++) {
            for (int j = 0; j < _massMatrix.Size; j++) {
                _massMatrix[i, j] = _grid.Omega * _grid.Sigma * Integration.GaussSegment(_basis[i].Invoke, _basis[j].Invoke,
                        new(0, 0, 0), new(1, 1, 1)) * (hx * hy * hz);
            }
        }
    }

    private void AssemblyLocalVector(int ielem) {
        for (int i = 0; i < _massMatrix.Size; i++) {
            for (int j = 0; j < _massMatrix.Size; j++) {
                _localVector1[i] += _test.Fs(_grid.Points[_grid.Elements[ielem][j]]) * _massMatrix[i, j] / (_grid.Omega * _grid.Sigma);
                _localVector2[i] += _test.Fc(_grid.Points[_grid.Elements[ielem][j]]) * _massMatrix[i, j] / (_grid.Omega * _grid.Sigma);
            }
        }
    }

    private void AccountingDirichletBoundary() {
        for (int iside = 0; iside < _grid.Sides.Length; iside++) {
            for (int inode = 0; inode < _grid.Sides[iside].Length; inode++) {
                _globalMatrix.di[2 * _grid.Sides[iside][inode]] = 1.0;
                _globalMatrix.di[(2 * _grid.Sides[iside][inode]) + 1] = 1.0;
                _vector[2 * _grid.Sides[iside][inode]] = _test.Us(_grid.Points[_grid.Sides[iside][inode]]);
                _vector[(2 * _grid.Sides[iside][inode]) + 1] = _test.Uc(_grid.Points[_grid.Sides[iside][inode]]);

                int diagonal = 2 * _grid.Sides[iside][inode];

                for (int k = _globalMatrix.ig[diagonal]; k < _globalMatrix.ig[diagonal + 1]; k++) {
                    _globalMatrix.ggl[k] = 0.0;
                }

                for (int i = diagonal + 1; i < _globalMatrix.Size; i++) {
                    for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                        if (_globalMatrix.jg[k] == diagonal) {
                            _globalMatrix.ggu[k] = 0.0;
                            break;
                        }
                    }
                }

                diagonal = (2 * _grid.Sides[iside][inode]) + 1;

                for (int k = _globalMatrix.ig[diagonal]; k < _globalMatrix.ig[diagonal + 1]; k++) {
                    _globalMatrix.ggl[k] = 0.0;
                }

                for (int i = diagonal + 1; i < _globalMatrix.Size; i++) {
                    for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                        if (_globalMatrix.jg[k] == diagonal) {
                            _globalMatrix.ggu[k] = 0.0;
                            break;
                        }
                    }
                }
            }
        }
    }

    //for report
    private void ErrIter() {
        using var sw = new StreamWriter("results/errIter.txt");
        for (int i = 0; i < _grid.Points.Length; i++) {
            sw.WriteLine(Math.Abs(_solver!.Solution!.Value[2 * i] - _test.Us(_grid.Points[i])));
            sw.WriteLine(Math.Abs(_solver.Solution.Value[(2 * i) + 1] - _test.Uc(_grid.Points[i])));
        }
    }

    private void ErrForward() {
        using var sw = new StreamWriter("results/errForward.txt");
        for (int i = 0; i < _grid.Points.Length; i++) {
            sw.WriteLine(Math.Abs(_decomposer!.Solution!.Value[2 * i] - _test.Us(_grid.Points[i])));
            sw.WriteLine(Math.Abs(_decomposer.Solution.Value[(2 * i) + 1] - _test.Uc(_grid.Points[i])));
        }
    }

    public static FEMBuilder CreateBuilder()
        => new();
}