namespace eMF_1;

public class RegularGrid : Grid
{
    private List<double> _allLinesX;
    private List<double> _allLinesY;
    private List<Point2D> _points;
    public override double[] LinesX { get; init; }
    public override double[] LinesY { get; init; }
    public override List<double> AllLinesX
        => _allLinesX;
    public override List<double> AllLinesY
        => _allLinesY;
    public override List<Point2D> Points
        => _points;
    public override double[][] Areas { get; init; }
    public int SplitsX { get; init; }
    public int SplitsY { get; init; }

    public RegularGrid(string path)
    {
        try
        {
            using (var sr = new StreamReader(path))
            {
                LinesX = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
                LinesY = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
                SplitsX = int.Parse(sr.ReadLine());
                SplitsY = int.Parse(sr.ReadLine());
                Areas = sr.ReadToEnd().Split("\n").Select(row => row.Split(" ")
                .Select(value => double.Parse(value)).ToArray()).ToArray();
            }

            _allLinesX = new();
            _allLinesY = new();
            _points = new();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public override void Build()
    {
        double h;
        double lenght = LinesX.Last() - LinesX.First();

        h = lenght / SplitsX;

        _allLinesX.Add(LinesX.First());

        while (Math.Round(_allLinesX.Last() + h, 1) < LinesX.Last())
            _allLinesX.Add(_allLinesX.Last() + h);

        _allLinesX = _allLinesX.Union(LinesX).OrderBy(value => value).ToList();

        lenght = LinesY.Last() - LinesY.First();

        h = lenght / SplitsY;

        _allLinesY.Add(LinesY.First());

        while (Math.Round(_allLinesY.Last() + h, 1) < LinesY.Last())
            _allLinesY.Add(_allLinesY.Last() + h);

        _allLinesY = _allLinesY.Union(LinesY).OrderBy(value => value).ToList();

        for (int i = 0; i < _allLinesX.Count; i++)
            for (int j = 0; j < _allLinesY.Count; j++)
                _points.Add(new(_allLinesX[i], _allLinesY[j], i, j, PointsTypes(_allLinesX[i], _allLinesY[j])));

        WriteToFilePoints();
    }

    private PointType PointsTypes(double x, double y)
    {
        double eps = 1E-14;

        if ((x > LinesX[0] && x < LinesX[2] && y > LinesY[0] && y < LinesY[1])
        || (x > LinesX[0] && x < LinesX[1] && y > LinesY[0] && y < LinesY[2])
        || (x == LinesX[1] && y == LinesY[1]))
            return PointType.Internal;

        for (int i = 0; i < _allLinesX.Count; i++)
            if ((Math.Abs(x - _allLinesX[i]) < eps && Math.Abs(y - LinesY[0]) < eps)
            || (Math.Abs(x - _allLinesX[i]) < eps && Math.Abs(y - LinesY[1]) < eps && x >= LinesX[1])
            || (Math.Abs(x - _allLinesX[i]) < eps && Math.Abs(y - LinesY[2]) < eps && x <= LinesX[1]))
                return PointType.Boundary;

        for (int i = 0; i < _allLinesY.Count; i++)
            if ((Math.Abs(y - _allLinesY[i]) < eps && Math.Abs(x - LinesX[0]) < eps)
            || (Math.Abs(y - _allLinesY[i]) < eps && Math.Abs(x - LinesX[1]) < eps && y >= LinesY[1])
            || (Math.Abs(y - _allLinesY[i]) < eps && Math.Abs(x - LinesX[2]) < eps && y <= LinesY[1]))
                return PointType.Boundary;

        return PointType.Dummy;
    }

    private void WriteToFilePoints()
    {
        using (var sw = new StreamWriter("points/boundaryPoints.txt"))
        {
            Points.ForEach(x => { if (x.PointType == PointType.Boundary) sw.WriteLine(x.ToString()); });
        }

        using (var sw = new StreamWriter("points/internalPoints.txt"))
        {
            Points.ForEach(x => { if (x.PointType == PointType.Internal) sw.WriteLine(x.ToString()); });
        }

        using (var sw = new StreamWriter("points/dummyPoints.txt"))
        {
            Points.ForEach(x => { if (x.PointType == PointType.Dummy) sw.WriteLine(x.ToString()); });
        }
    }
}