namespace eMF_1;

public abstract class Grid
{
    public abstract double[] LinesX { get; init; }
    public abstract double[] LinesY { get; init; }
    public abstract List<double> AllLinesX { get; }
    public abstract List<double> AllLinesY { get; }
    public abstract List<Point2D> Points { get;}
    public abstract double[][] Areas { get; init; }

    public abstract void Build();
}