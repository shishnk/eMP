namespace eMP_2;

public readonly record struct Point2D(double X, double Y) {
    public static Point2D Parse(string points) {
        var data = points.Split(" ");
        Point2D point = new(double.Parse(data[0]), double.Parse(data[1]));

        return point;
    }

    public override string ToString() {
        return $"({X},{Y})";
    }
}