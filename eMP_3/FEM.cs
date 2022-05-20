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

        public static implicit operator FEM(FEMBuilder builder)
            => builder._fem;
    }

    // default! указывает на то, что данное поле не может принимать null
    private delegate double Basis(Point3D point);
    private Basis[] _basis = default!;
    private ITest _test = default!;
    private Grid _grid = default!;
    private Solver _solver = default!;
    private Matrix _massMatrix = default!; // матрица масс
    private Matrix _stiffnessMatrix = default!; // матрица жесткости
    private SparseMatrix _globalMatrix = default!;
    private Vector<double> _localVector1 = default!;
    private Vector<double> _localVector2 = default!;
    private Vector<double> _vector = default!; // вектор правой части

    public void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");
            ArgumentNullException.ThrowIfNull(_solver, $"{nameof(_solver)} cannot be null, set the solver SLAE");

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

        _solver.SetMatrix(_globalMatrix);
        _solver.SetVector(_vector);
        _solver.Compute();

        foreach (var q in _solver.Solution!) {
            Console.WriteLine(q);
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

        _globalMatrix.ig[0] = 0;
        _globalMatrix.ig[1] = 0;
        _globalMatrix.ig[2] = 1;

        for (int i = 1; i < list.Length; i++) {
            _globalMatrix.ig[(2 * i) + 1] = _globalMatrix.ig[2 * i] + (2 * list[i].Count);
            _globalMatrix.ig[(2 * i) + 2] = _globalMatrix.ig[(2 * i) + 1] + (2 * list[i].Count) + 1;
        }

        int index = 0;

        _globalMatrix.jg = new int[_globalMatrix.ig[^1]];
        _globalMatrix.ggl = new double[_globalMatrix.ig[^1]];
        _globalMatrix.ggu = new double[_globalMatrix.ig[^1]];

        for (int i = 1; i < _globalMatrix.Size; i++) {
            int count = 0;

            for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                _globalMatrix.jg[index++] = count++;
            }
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
                _stiffnessMatrix[i, j] = (Integration.GaussSegmentGrad(_basis[i].Invoke, _basis[j].Invoke, new(0, 0, 0), new(1, 1, 1)) /
                (hx * hy * hz)) - (Integration.GaussSegment(_basis[i].Invoke, _basis[j].Invoke, new(0, 0, 0), new(1, 1, 1)) * (hx * hy * hz));
            }
        }

        for (int i = 0; i < _massMatrix.Size; i++) {
            for (int j = 0; j < _massMatrix.Size; j++) {
                _massMatrix[i, j] = Integration.GaussSegment(_basis[i].Invoke, _basis[j].Invoke,
                        new(0, 0, 0), new(1, 1, 1)) * (hx * hy * hz);
            }
        }
    }

    private void AssemblyLocalVector(int ielem) {
        for (int i = 0; i < _massMatrix.Size; i++) {
            for (int j = 0; j < _massMatrix.Size; j++) {
                _localVector1[i] += _test.Fs(_grid.Points[(2 * ielem) + j]) * _massMatrix[i, j];
                _localVector2[i] += _test.Fc(_grid.Points[(2 * ielem) + j]) * _massMatrix[i, j];
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

                int count = 2 * _grid.Sides[iside][inode];

                for (int k = _globalMatrix.ig[2 * _grid.Sides[iside][inode]]; k < _globalMatrix.ig[(2 * _grid.Sides[iside][inode]) + 1]; k++) {
                    _globalMatrix.ggl[k] = 0.0;
                }

                for (int i = (2 * _grid.Sides[iside][inode]) + 1; i < _globalMatrix.Size; i++) {
                    int check = 0;

                    for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                        if (check == count) {
                            _globalMatrix.ggu[k] = 0.0;
                            break;
                        }

                        check++;
                    }
                }

                count = (2 * _grid.Sides[iside][inode]) + 1;

                for (int k = _globalMatrix.ig[(2 * _grid.Sides[iside][inode]) + 1]; k < _globalMatrix.ig[(2 * _grid.Sides[iside][inode]) + 2]; k++) {
                    _globalMatrix.ggl[k] = 0.0;
                }

                for (int i = (2 *_grid.Sides[iside][inode]) + 2; i < _globalMatrix.Size; i++) {
                    int check = 0;

                    for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                        if (check == count) {
                            _globalMatrix.ggu[k] = 0.0;
                            break;
                        }

                        check++;
                    }
                }

                // _globalMatrix.PrintDense("kek.txt");
            }
        }
    }

    public static FEMBuilder CreateBuilder()
        => new();
}