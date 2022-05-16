namespace eMP_3;

public readonly record struct Point3D(double X, double Y, double Z) {
    public static Point3D operator +(Point3D point, (double, double, double) value)
        => new(point.X + value.Item1, point.Y + value.Item2, point.Z + value.Item3);

    public static Point3D operator -(Point3D point, (double, double, double) value)
        => new(point.X - value.Item1, point.Y - value.Item2, point.Z - value.Item3);

    public override string ToString()
        => $"{X} {Y} {Z}";
}
