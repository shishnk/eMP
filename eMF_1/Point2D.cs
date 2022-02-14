namespace eMF_1;

public record struct Point2D
{
    public double X { get; init; }
    public double Y { get; init; }
    public PointType PointType { get; set; }
    public BoundaryType BoundaryType { get; set; } = BoundaryType.Dirichlet;

    public Point2D(double x, double y, PointType pointType)
    {
        X = x;
        Y = y;
        PointType = pointType;
        BoundaryType = BoundaryType.Dirichlet;
    }

    public static Point2D Parse(string pointStr)
    {
        var data = pointStr.Split();
        Point2D point = new(double.Parse(data[0]), double.Parse(data[1]),
        (PointType)Enum.Parse(typeof(PointType), data[2]));

        return point;
    }

    public override string ToString()
        => $"{X} {Y}";
}