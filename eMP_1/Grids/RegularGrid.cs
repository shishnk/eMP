namespace eMF_1;

public class RegularGrid : Grid {
    private List<double> _allLinesX;
    private List<double> _allLinesY;
    private readonly List<Point2D> _points;
    private readonly (int, double, double, int, int, int, int)[] _areas;
    public override ImmutableArray<double> LinesX { get; init; }
    public override ImmutableArray<double> LinesY { get; init; }
    public override ImmutableList<double> AllLinesX
        => _allLinesX.ToImmutableList();
    public override ImmutableList<double> AllLinesY
        => _allLinesY.ToImmutableList();
    public override ImmutableList<Point2D> Points
        => _points.ToImmutableList();
    public override ImmutableArray<(int, double, double, int, int, int, int)> Areas
        => _areas.ToImmutableArray();
    public int SplitsX { get; init; }
    public int SplitsY { get; init; }

    public RegularGrid(string path) {
        try {
            using (var sr = new StreamReader(path)) {
                LinesX = sr.ReadLine().Split().Select(value => double.Parse(value)).ToImmutableArray();
                LinesY = sr.ReadLine().Split().Select(value => double.Parse(value)).ToImmutableArray();
                SplitsX = int.Parse(sr.ReadLine());
                SplitsY = int.Parse(sr.ReadLine());
                _areas = sr.ReadToEnd().Split("\n").Select(row => row.Split())
                .Select(value => (int.Parse(value[0]), double.Parse(value[1]), double.Parse(value[2]),
                int.Parse(value[3]), int.Parse(value[4]), int.Parse(value[5]), int.Parse(value[6]))).ToArray();
            }

            _allLinesX = new();
            _allLinesY = new();
            _points = new();
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    public override void Build() {
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
                PointTypesWithoutInternalCheck(_allLinesX[i], _allLinesY[j])));

        InternalCheck();
        SetAreaNumber();
        WriteToFilePoints();
    }
}