namespace eMF_1;

public class IrregularGrid : Grid
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
    public override double[][] Areas { get; init; }
    public int[] SplitsX { get; init; }
    public int[] SplitsY { get; init; }
    public double[] KX { get; init; }
    public double[] KY { get; init; }

    public IrregularGrid(string path)
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
        for (int i = 0; i < LinesX.Length - 1; i++)
        {
            double h;
            double sum = 0;
            double lenght = LinesX[i + 1] - LinesX[i];

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
                _points.Add(new(_allLinesX[i], _allLinesY[j], i, j, PointsTypes(_allLinesX[i], _allLinesY[j])));

        WriteToFilePoints();
    }

    private PointType PointsTypes(double x, double y)
    {
        double eps = 1E-8;

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
            Points.ForEach(x => { if (x.PointType == PointType.Boundary) sw.WriteLine(x); });
        }

        using (var sw = new StreamWriter("points/internalPoints.txt"))
        {
            Points.ForEach(x => { if (x.PointType == PointType.Internal) sw.WriteLine(x); });
        }

        using (var sw = new StreamWriter("points/dummyPoints.txt"))
        {
            Points.ForEach(x => { if (x.PointType == PointType.Dummy) sw.WriteLine(x); });
        }
    }
}