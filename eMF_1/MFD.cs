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

public enum NormalType : sbyte
{
    LeftX = -1,
    RightX = 1,
    UpperY = 1,
    BottomY = -1
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
            _grid.AssignBoundaryConditions(_boundaries);
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
        _matrix = new(_grid.Points.Count, _grid.AllLinesX.Count - 2);
        _pr = new double[_matrix.Size];
        _q = new double[_matrix.Size];
    }

    private void BuildMatrix()
    {
        double h = 1E-14;

        for (int i = 0; i < _grid.Points.Count; i++)
        {
            switch (_grid.Points[i].PointType)
            {
                case PointType.Boundary:

                    switch (_grid.Points[i].BoundaryType)
                    {
                        case BoundaryType.Dirichlet:

                            _matrix.Diags[0][i] = 1;
                            _pr[i] = _test.U(_grid.Points[i]);
                            Console.WriteLine($"{_grid.Points[i]} normal is {_grid.Normal(_grid.Points[i])}");

                            break;

                        case BoundaryType.Neumann:

                            // double lambda = _grid.Areas[_grid.Points[i].AreaNumber].Item2;

                            // _matrix.Diags[0][i] =

                            break;

                        case BoundaryType.Mixed:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(BoundaryType),
                            $"This type of boundary does not exist: {_grid.Points[i].BoundaryType}");
                    }

                    break;

                case PointType.Internal:

                    double hx = _grid.AllLinesX[_grid.Points[i].I + 1] - _grid.AllLinesX[_grid.Points[i].I];
                    double hy = _grid.AllLinesY[_grid.Points[i].J + 1] - _grid.AllLinesY[_grid.Points[i].J];
                    (double lambda, double gamma) =
                    (_grid.Areas[_grid.Points[i].AreaNumber].Item2,
                    _grid.Areas[_grid.Points[i].AreaNumber].Item3);
                    _pr[i] = _test.F(_grid.Points[i]);
                    _matrix.Diags[0][i] = lambda * (2.0 / (hx * hx) + 2.0 / (hy * hy)) + gamma;
                    _matrix.Diags[3][i] = -lambda / (hy * hy);
                    _matrix.Diags[4][i] = -lambda / (hx * hx);
                    _matrix.Diags[1][i + _matrix.Indexes[1]] = -lambda / (hy * hy);
                    _matrix.Diags[2][i + _matrix.Indexes[2]] = -lambda / (hx * hx);

                    break;

                case PointType.Dummy:

                    _matrix.Diags[0][i] = 1;
                    _pr[i] = 0;

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(PointType),
                    $"This type of point does not exist: {_grid.Points[i].PointType}");
            }
        }

        Console.WriteLine("\n\n");
    }
}