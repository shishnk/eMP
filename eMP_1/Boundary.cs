namespace eMP_1;

public readonly record struct Boundary(BoundaryType BoundaryType, int X1, int X2, int Y1, int Y2) {
    public static Boundary BoundaryParse(string boundaryStr) {
        var data = boundaryStr.Split();
        Boundary boundary = new((BoundaryType)Enum.Parse(typeof(BoundaryType), data[0]),
        int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]));

        return boundary;
    }
}