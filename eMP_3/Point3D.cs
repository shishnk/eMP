namespace eMP_3;

public readonly record struct Point3D(double X, double Y, double Z) {
    public override string ToString()
        => $"{X} {Y} {Z}";
}
