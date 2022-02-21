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

public enum NormalType
{
    LeftX,
    RightX,
    UpperY,
    BottomY
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
    private double _beta;
    public double[] Weights
        => _q;

    public MFD(Grid grid, string boundaryPath)
    {
        try
        {
            using (var sr = new StreamReader(boundaryPath))
            {
                _beta = double.Parse(sr.ReadLine());
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
        _matrix = new(_grid.Points.Count, (_grid.AllLinesX.Count > _grid.AllLinesX.Count) ?
        _grid.AllLinesX.Count - 2 : _grid.AllLinesY.Count - 2);
        _pr = new double[_matrix.Size];
        _q = new double[_matrix.Size];
    }

    private void BuildMatrix()
    {
        double hx, hy, hix, hiy, hi, h = 1E-12;
        double lambda, gamma;
        double us, ubeta;
        double leftDerivative, rightDerivative;
        NormalType normalType;

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

                            break;

                        case BoundaryType.Neumann:

                            lambda = _grid.Areas[_grid.Points[i].AreaNumber].Item2;

                            normalType = _grid.Normal(_grid.Points[i]);

                            switch (normalType)
                            {
                                case NormalType.LeftX:

                                    hi = _grid.AllLinesX[_grid.Points[i].I + 1] - _grid.AllLinesX[_grid.Points[i].I];
                                    _matrix.Diags[0][i] = lambda / hi;
                                    _matrix.Diags[4][i] = -lambda / hi;
                                    _pr[i] = -lambda * RightDerivativeX(_grid.Points[i], h);

                                    break;

                                case NormalType.BottomY:

                                    hi = _grid.AllLinesY[_grid.Points[i].J + 1] - _grid.AllLinesY[_grid.Points[i].J];
                                    _matrix.Diags[0][i] = lambda / hi;
                                    _matrix.Diags[3][i] = -lambda / hi;
                                    _pr[i] = -lambda * RightDerivativeY(_grid.Points[i], h);

                                    break;

                                case NormalType.RightX:

                                    hi = _grid.AllLinesX[_grid.Points[i].I] - _grid.AllLinesX[_grid.Points[i].I - 1];
                                    _matrix.Diags[0][i] = lambda / hi;
                                    _matrix.Diags[2][i + _matrix.Indexes[2]] = -lambda / hi;
                                    _pr[i] = lambda * LeftDerivativeX(_grid.Points[i], h);

                                    break;

                                case NormalType.UpperY:

                                    hi = _grid.AllLinesY[_grid.Points[i].J] - _grid.AllLinesY[_grid.Points[i].J - 1];
                                    _matrix.Diags[0][i] = lambda / hi;
                                    _matrix.Diags[1][i + _matrix.Indexes[1]] = -lambda / hi;
                                    _pr[i] = lambda * LeftDerivativeY(_grid.Points[i], h);

                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(normalType),
                                    $"This type of normal does not exist: {normalType}");
                            }

                            break;

                        case BoundaryType.Mixed:

                            lambda = _grid.Areas[_grid.Points[i].AreaNumber].Item2;

                            normalType = _grid.Normal(_grid.Points[i]);
                            us = _test.U(_grid.Points[i]);

                            switch (normalType)
                            {
                                case NormalType.LeftX:

                                    hi = _grid.AllLinesX[_grid.Points[i].I + 1] - _grid.AllLinesX[_grid.Points[i].I];
                                    rightDerivative = RightDerivativeX(_grid.Points[i], h);
                                    ubeta = -lambda * rightDerivative / _beta + us;
                                    _matrix.Diags[0][i] = lambda / hi + _beta;
                                    _matrix.Diags[4][i] = -lambda / hi;
                                    _pr[i] = -lambda * rightDerivative + _beta * (us - ubeta) + _beta * ubeta;

                                    break;

                                case NormalType.BottomY:

                                    hi = _grid.AllLinesY[_grid.Points[i].J + 1] - _grid.AllLinesY[_grid.Points[i].J];
                                    rightDerivative = RightDerivativeY(_grid.Points[i], h);
                                    ubeta = -lambda * rightDerivative / _beta + us;
                                    _matrix.Diags[0][i] = lambda / hi + _beta;
                                    _matrix.Diags[3][i] = -lambda / hi;
                                    _pr[i] = -lambda * rightDerivative + _beta * (us - ubeta) + _beta * ubeta;

                                    break;

                                case NormalType.RightX:

                                    hi = _grid.AllLinesX[_grid.Points[i].I] - _grid.AllLinesX[_grid.Points[i].I - 1];
                                    leftDerivative = LeftDerivativeX(_grid.Points[i], h);
                                    ubeta = lambda * leftDerivative / _beta + us;
                                    _matrix.Diags[0][i] = lambda / hi + _beta;
                                    _matrix.Diags[2][i + _matrix.Indexes[2]] = -lambda / hi;
                                    _pr[i] = lambda * leftDerivative + _beta * (us - ubeta) + _beta * ubeta;

                                    break;

                                case NormalType.UpperY:

                                    hi = _grid.AllLinesY[_grid.Points[i].J] - _grid.AllLinesY[_grid.Points[i].J - 1];
                                    leftDerivative = LeftDerivativeY(_grid.Points[i], h);
                                    ubeta = lambda * leftDerivative / _beta + us;
                                    _matrix.Diags[0][i] = lambda / hi + _beta;
                                    _matrix.Diags[1][i + _matrix.Indexes[1]] = -lambda / hi;
                                    _pr[i] = lambda * leftDerivative + _beta * (us - ubeta) + _beta * ubeta;

                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(normalType),
                                    $"This type of normal does not exist: {normalType}");
                            }

                            break;


                        default:
                            throw new ArgumentOutOfRangeException(nameof(BoundaryType),
                            $"This type of boundary does not exist: {_grid.Points[i].BoundaryType}");
                    }

                    break;

                case PointType.Internal:

                    hx = _grid.AllLinesX[_grid.Points[i].I + 1] - _grid.AllLinesX[_grid.Points[i].I];
                    hy = _grid.AllLinesY[_grid.Points[i].J + 1] - _grid.AllLinesY[_grid.Points[i].J];

                    (lambda, gamma) =
                    (_grid.Areas[_grid.Points[i].AreaNumber].Item2,
                    _grid.Areas[_grid.Points[i].AreaNumber].Item3);

                    if (_grid is RegularGrid)
                    {
                        _pr[i] = _test.F(_grid.Points[i]);
                        _matrix.Diags[0][i] = lambda * (2.0 / (hx * hx) + 2.0 / (hy * hy)) + gamma;
                        _matrix.Diags[3][i] = -lambda / (hy * hy);
                        _matrix.Diags[4][i] = -lambda / (hx * hx);
                        _matrix.Diags[1][i + _matrix.Indexes[1]] = -lambda / (hy * hy);
                        _matrix.Diags[2][i + _matrix.Indexes[2]] = -lambda / (hx * hx);
                    }
                    else
                    {
                        hix = _grid.AllLinesX[_grid.Points[i].I] - _grid.AllLinesX[_grid.Points[i].I - 1];
                        hiy = _grid.AllLinesY[_grid.Points[i].J] - _grid.AllLinesX[_grid.Points[i].J - 1];

                        _pr[i] = _test.F(_grid.Points[i]);
                        _matrix.Diags[0][i] = lambda * (2 / (hix * hx) + 2 / (hiy * hy)) + gamma;
                        _matrix.Diags[2][i + _matrix.Indexes[2]] = -lambda * 2 / (hix * (hx + hix));
                        _matrix.Diags[1][i + _matrix.Indexes[1]] = -lambda * 2 / (hiy * (hy + hiy));
                        _matrix.Diags[4][i] = -lambda * 2 / (hx * (hx + hix));
                        _matrix.Diags[3][i] = -lambda * 2 / (hy * (hy + hiy));
                    }

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
    }

    private double LeftDerivativeX(Point2D point, double h)
        => (_test.U(point) - _test.U(point - (h, 0))) / h;

    private double LeftDerivativeY(Point2D point, double h)
        => (_test.U(point) - _test.U(point - (0, h))) / h;

    private double RightDerivativeX(Point2D point, double h)
        => (_test.U(point + (h, 0)) - _test.U(point)) / h;

    private double RightDerivativeY(Point2D point, double h)
        => (_test.U(point + (0, h)) - _test.U(point)) / h;
}