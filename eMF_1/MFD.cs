namespace eMF_1;

public enum PointType
{
    Boundary,
    Internal,
    Dummy
}

public enum BoundaryType
{
    None,
    Dirichlet,
    Neumann,
    Mixed
}

public enum GridType
{
    Regular,
    Irregular
}

public class MFD
{
    private Grid _grid;
    private DiagMatrix _matrix;
    private ITest _test;
    private ISolver _solver;
    private Boundary[] _boundaries;
    private double[] _q;
    private double[] _pr;
    public double[] Weights
        => _q;

    public MFD(Grid grid, string boundaryPath)
    {
        try
        {
            using (var sr = new StreamReader(boundaryPath))
            {
                _boundaries = sr.ReadToEnd().Split("\n")
                .Select(str => Boundary.BoundaryParse(str)).ToArray();
            }

            _grid = grid;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void SetTest(ITest test)
        => _test = test;

    public void SetMethodSolvingSLAE(ISolver solver)
        => _solver = solver;

    public void Compute()
    {
        try
        {
            if (_test is null)
                throw new Exception("Set the test!");

            if (_solver is null)
                throw new Exception("Set the method solving SLAE!");

            _grid.Build();
            Init();
            BuildMatrix();
            _q = _solver.Compute(_matrix, _pr);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void Init()
    {
        _matrix = new(_grid.Points.Count, _grid.LinesX.Length - 1);
        _pr = new double[_matrix.Size];
        _q = new double[_matrix.Size];
    }

    private void BuildMatrix() // TODO (разобраться с проблемой подсчета внутреннего узла, добавить краевые условия)
    {
        for (int i = 0; i < _grid.Points.Count; i++)
        {
            switch (_grid.Points[i].PointType)
            {
                case PointType.Boundary:

                    _matrix.Diags[0][i] = 1;
                    _pr[i] = _test.U(_grid.Points[i]);

                    break;

                case PointType.Internal:

                    double hx = _grid.AllLinesX[_grid.Points[i].I + 1] - _grid.AllLinesX[_grid.Points[i].I];
                    double hy = _grid.AllLinesY[_grid.Points[i].J + 1] - _grid.AllLinesY[_grid.Points[i].J];
                    _pr[i] = _test.F(_grid.Points[i]);
                    _matrix.Diags[0][i] = -(2.0 / (hx * hx) + 2.0 / (hy * hy));
                    _matrix.Diags[1][i - 1] = 1.0 / (hx * hx);
                    _matrix.Diags[3][i] = 1.0 / (hx * hx);
                    _matrix.Diags[2][i - 3] = 1.0 / (hy * hy);
                    _matrix.Diags[4][i] = 1.0 / (hy * hy);

                    break;

                case PointType.Dummy:

                    _matrix.Diags[0][i] = 1;
                    _pr[i] = 0;

                    break;

                default:
                    break;
            }
        }
    }
}