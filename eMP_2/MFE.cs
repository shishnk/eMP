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

        public MFEBuilder SetMethod(ISolver solver) {
            _mfe._solver = solver;
            return this;
        }

        public static implicit operator MFE(MFEBuilder builder)
            => builder._mfe;
    }
    // default! указывает на то, что данное поле не может принимать null
    public delegate double Basis(double x);

    private Integration _integration = new();
    private Basis[] _basis = default!;
    private Basis[] _dBasis = default!;
    private ITest _test = default!;
    private ISolver _solver = default!;
    private IGrid _spaceGrid = default!;
    private IGrid _timeGrid = default!;
    private Matrix M = default!; // матрица масс
    private Matrix? G; // матрица жесткости
    private FiniteElement[] _elements = default!;
    private double[] q = default!; // вектор весов
    private double[] f = default!; // вектор правой части

    public void Compute() {
        try { // не забыть раскомментировать!
            // ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test"); 
            // ArgumentNullException.ThrowIfNull(_solver, $"{nameof(_solver)} cannot be null, set the method of solving SLAE");

            Init();
            Console.WriteLine();


        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    private void Init() {
        M = new(3);
        G = new(3);
        _basis = new Basis[] { QuadraticBasis.Psi1, QuadraticBasis.Psi2, QuadraticBasis.Psi3 };
        _dBasis = new Basis[] { QuadraticBasis.DPsi1, QuadraticBasis.DPsi2, QuadraticBasis.DPsi3 };
        _integration = new();
        _elements = new FiniteElement[_spaceGrid.Points.Length - 1];

        int index = 0;

        for (int ielem = 0; ielem < _elements.Length; ielem++) { // формирование конечных элементов
            _elements[ielem] = new(new(_spaceGrid.Points[index], _spaceGrid.Points[index + 1]));
            index++;
        }

        // for (int i = 0; i < M.Size; i++) // рабочий алгоритм взятия интегралов от базисных функций на шаблонном элементе
        //     for (int j = 0; j < M.Size; j++)
        //         M[i, j] = _integration.GaussOrder5(_dBasis[i], _dBasis[j]) / _elements[0].Interval.Lenght;
    }

    private void BuildLocalMatrix() {

    }

    private void BuildGlobalMatrix() {

    }

    private void BuildF() {

    }

    public static MFEBuilder CreateBuilder()
        => new();
}