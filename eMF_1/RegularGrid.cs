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
    public override (int, double, double, int, int, int, int)[] Areas { get; init; }
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
                Areas = sr.ReadToEnd().Split("\n").Select(row => row.Split())
                .Select(value => (int.Parse(value[0]), double.Parse(value[1]), double.Parse(value[2]),
                int.Parse(value[3]), int.Parse(value[4]), int.Parse(value[5]), int.Parse(value[6]))).ToArray();
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
                _points.Add(new(_allLinesX[i], _allLinesY[j], i, j,
                PointsTypes(_allLinesX[i], _allLinesY[j])));

        SetAreaNumber();
        WriteToFilePoints();
    }
}