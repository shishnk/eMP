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

        public MFEBuilder SetGrid(Grid grid) {
            _mfe._grid = grid;
            return this;
        }

        public MFEBuilder SetMethod(ISolver solver) {
            _mfe._solver = solver;
            return this;
        }

        public static implicit operator MFE(MFEBuilder builder)
            => builder._mfe;
    }

    private ITest? _test;
    private ISolver? _solver;
    private Grid? _grid;

    public void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_test, $"{nameof(_test)} cannot be null, set the test");
            ArgumentNullException.ThrowIfNull(_solver, $"{nameof(_solver)} cannot be null, set the method of solving SLAE");

        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    public static MFEBuilder CreateBuilder()
        => new();
}