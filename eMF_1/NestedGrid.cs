namespace eMF_1;

public class NestedGrid : Grid
{
    private List<double> _allLinesX;
    private List<double> _allLinesY;
    private List<Point2D> _points;
    public override double[] LinesX { get; init; }
    public override double[] LinesY { get; init; }
    public override List<Point2D> Points
     => _points;
    public override List<double> AllLinesX
        => _allLinesX;
    public override List<double> AllLinesY
        => _allLinesY;
    public override (int, double, double, int, int, int, int)[] Areas { get; init; }
    public int[] SplitsX { get; init; }
    public int[] SplitsY { get; init; }
    public double[] KX { get; init; }
    public double[] KY { get; init; }

    public NestedGrid(string path)
    {
        try
        {
            using (var sr = new StreamReader(path))
            {
                LinesX = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
                LinesY = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
                SplitsX = sr.ReadLine().Split().Select(value => int.Parse(value)).ToArray();
                SplitsY = sr.ReadLine().Split().Select(value => int.Parse(value)).ToArray();
                KX = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
                KY = sr.ReadLine().Split().Select(value => double.Parse(value)).ToArray();
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
        for (int i = 0; i < LinesX.Length - 1; i++)
        {
            double h;
            double sum = 0;
            double lenght = LinesX[i + 1] - LinesX[i];

            for (int j = 0; j < KX.Length; j++)
                KX[j] = Math.Sqrt(KX[j]);

            for (int k = 0; k < SplitsX[i]; k++)
                sum += Math.Pow(KX[i], k);

            h = lenght / sum;

            _allLinesX.Add(LinesX[i]);

            while (Math.Round(_allLinesX.Last() + h, 1) < LinesX[i + 1])
            {
                _allLinesX.Add(_allLinesX.Last() + h);

                h *= KX[i];
            }

            sum = 0;

        }

        _allLinesX.Add(LinesX.Last());

        for (int i = 0; i < LinesY.Length - 1; i++)
        {
            double h;
            double sum = 0;
            double lenght = LinesY[i + 1] - LinesY[i];

            for (int j = 0; j < KY.Length; j++)
                KY[j] = Math.Sqrt(KY[j]);

            for (int k = 0; k < SplitsY[i]; k++)
                sum += Math.Pow(KY[i], k);

            h = lenght / sum;

            _allLinesY.Add(LinesY[i]);

            while (Math.Round(_allLinesY.Last() + h, 1) < LinesY[i + 1])
            {
                _allLinesY.Add(_allLinesY.Last() + h);

                h *= KY[i];
            }

            sum = 0;
        }

        _allLinesY.Add(LinesY.Last());

        for (int i = 0; i < _allLinesX.Count; i++)
            for (int j = 0; j < _allLinesY.Count; j++)
                _points.Add(new(_allLinesX[i], _allLinesY[j], i, j,
                PointsTypes(_allLinesX[i], _allLinesY[j])));

        SetAreaNumber();
        WriteToFilePoints();
    }
}