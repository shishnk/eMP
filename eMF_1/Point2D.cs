namespace eMF_1;

public class Point2D
{
    public double X { get; init; }
    public double Y { get; init; }
    public int I { get; init; }
    public int J { get; init; }
    public PointType PointType { get; init; }
    public BoundaryType BoundaryType { get; set; } = BoundaryType.None;
    public int AreaNumber { get; set; }

    public Point2D(double x, double y, int i, int j, PointType pointType)
    {
        X = x;
        Y = y;
        I = i;
        J = j;
        PointType = pointType;
    }

    public static Point2D Parse(string pointStr)
    {
        var data = pointStr.Split();
        Point2D point = new(double.Parse(data[0]), double.Parse(data[1]),
        int.Parse(data[2]), int.Parse(data[3]), (PointType)Enum.Parse(typeof(PointType), data[4]));

        return point;
    }

    public override string ToString()
        => $"{X} {Y}";
}