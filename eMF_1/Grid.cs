namespace eMF_1;

public class Grid
{
    public double[] LinesX { get; init; }
    public double[] LinesY { get; init; }
    public int[] SplitsX { get; init; }
    public int[] SplitsY { get; init; }
    public double[] KX { get; init; }
    public double[] KY { get; init; }
    public List<Point2D> Points { get; init; }
    public double[][] Areas { get; init; }
    public double[][] Boundaries { get; init; }
    public List<double> AllLinesX { get; private set; }
    public List<double> AllLinesY { get; private set; }

    public Grid(string areasPath, string boundariesPath)
    {
        try
        {
            using (var sr = new StreamReader(areasPath))
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

            using (var sr = new StreamReader(boundariesPath))
            {
                Boundaries = sr.ReadToEnd().Split("\n").Select(str => str.Split()
                .Select(value => double.Parse(value)).ToArray()).ToArray();
            }

            AllLinesX = new();
            AllLinesY = new();
            Points = new();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Build()
    {
        for (int i = 0; i < LinesX.Length - 1; i++)
        {
            double h;
            double sum = 0;
            double lenght = LinesX[i + 1] - LinesX[i];

            for (int k = 0; k < SplitsX[i]; k++)
                sum += Math.Pow(KX[i], k);

            h = lenght / sum;

            AllLinesX.Add(LinesX[i]);

            while (Math.Round(AllLinesX.Last() + h, 1) < LinesX[i + 1])
            {
                AllLinesX.Add(AllLinesX.Last() + h);

                h *= KX[i];
            }

            sum = 0;

        }

        AllLinesX.Add(LinesX.Last());


        for (int i = 0; i < LinesY.Length - 1; i++)
        {
            double h;
            double sum = 0;
            double lenght = LinesY[i + 1] - LinesY[i];

            for (int k = 0; k < SplitsY[i]; k++)
                sum += Math.Pow(KY[i], k);

            h = lenght / sum;

            AllLinesY.Add(LinesY[i]);

            while (Math.Round(AllLinesY.Last() + h, 1) < LinesY[i + 1])
            {
                AllLinesY.Add(AllLinesY.Last() + h);

                h *= KY[i];
            }

            sum = 0;
        }

        AllLinesY.Add(LinesY.Last());

        for (int i = 0; i < AllLinesX.Count; i++)
            for (int j = 0; j < AllLinesY.Count; j++)
                Points.Add(new(AllLinesX[i], AllLinesY[j], PointsTypes(AllLinesX[i], AllLinesY[j])));

        WriteToFilePoints();
    }

    private PointType PointsTypes(double x, double y)
    {
        double eps = 1E-8;

        if ((x > LinesX[0] && x < LinesX[2] && y > LinesY[0] && y < LinesY[1])
        || (x > LinesX[0] && x < LinesX[1] && y > LinesY[0] && y < LinesY[2]))
            return PointType.Internal;

        for (int i = 0; i < AllLinesX.Count; i++)
            if ((Math.Abs(x - AllLinesX[i]) < eps && Math.Abs(y - LinesY[0]) < eps)
            || (Math.Abs(x - AllLinesX[i]) < eps && Math.Abs(y - LinesY[1]) < eps && x >= LinesX[1])
            || (Math.Abs(x - AllLinesX[i]) < eps && Math.Abs(y - LinesY[2]) < eps && x <= LinesX[1]))
                return PointType.Boundary;

        for (int i = 0; i < AllLinesY.Count; i++)
            if ((Math.Abs(y - AllLinesY[i]) < eps && Math.Abs(x - LinesX[0]) < eps)
            || (Math.Abs(y - AllLinesY[i]) < eps && Math.Abs(x - LinesX[1]) < eps && y >= LinesY[1])
            || (Math.Abs(y - AllLinesY[i]) < eps && Math.Abs(x - LinesX[2]) < eps && y <= LinesY[1]))
                return PointType.Boundary;

        return PointType.Dummy;
    }

    private void WriteToFilePoints()
    {
        using (var sw = new StreamWriter("boundaryPoints.txt"))
        {

            Points.ForEach(x => { if (x.PointType == PointType.Boundary) sw.WriteLine(x.ToString()); });
        }

        using (var sw = new StreamWriter("internalPoints.txt"))
        {

            Points.ForEach(x => { if (x.PointType == PointType.Internal) sw.WriteLine(x.ToString()); });
        }

        using (var sw = new StreamWriter("dummyPoints.txt"))
        {

            Points.ForEach(x => { if (x.PointType == PointType.Dummy) sw.WriteLine(x.ToString()); });
        }
    }
}