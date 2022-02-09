namespace eMF_1;

public readonly record struct Point2D(double X, double Y, TypePoint typePoint, TypeBoundary TypeBoundary)
{
    public static Point2D Parse(string pointStr)
    {
        var data = pointStr.Split();
        Point2D point = new(double.Parse(data[0]), double.Parse(data[1]), 
        (TypePoint)Enum.Parse(typeof(TypePoint), data[2]), 
        (TypeBoundary)Enum.Parse(typeof(TypeBoundary), data[3]));

        return point; 
    }
}