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

        public static implicit operator FEM(FEMBuilder builder)
            => builder._fem;
    }

    // default! указывает на то, что данное поле не может принимать null

    private ITest _test = default!;
    private Grid _grid = default!;
    private Matrix _massMatrix = default!; // матрица масс
    private Matrix _stiffnessMatrix = default!; // матрица жесткости
    private SparseMatrix _globalMatrix = default!;
    private Vector<double> _localVector = default!;
    private Vector<double> _vector = default!; // вектор правой части

    public void Compute() {
        try {
            // ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");

            Init();
            Solve();
        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void Init() {
        _massMatrix = new(8);
        _stiffnessMatrix = new(8);
        _localVector = new(8);
    }

    private void InitSLAE(int sizeOffDiag) {
        _globalMatrix = new(2 * _grid.Points.Length, sizeOffDiag); // resizing in method ConstructPortrait
        _vector = new(2 * _grid.Points.Length);
    }

    private void Solve() {
        ConstructPortrait();
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
        _globalMatrix.gg = new double[_globalMatrix.ig[^1]];

        for (int i = 1; i < _globalMatrix.Size; i++) {
            int count = 0;

            for (int k = _globalMatrix.ig[i]; k < _globalMatrix.ig[i + 1]; k++) {
                _globalMatrix.jg[index++] = count;
                count++;
            }
        }
    }
    public static FEMBuilder CreateBuilder()
        => new();
}