namespace eMP_1;

public abstract class Grid {
    public abstract ImmutableArray<double> LinesX { get; init; }
    public abstract ImmutableArray<double> LinesY { get; init; }
    public abstract ImmutableList<double> AllLinesX { get; }
    public abstract ImmutableList<double> AllLinesY { get; }
    public abstract ImmutableList<Point2D> Points { get; }
    public abstract ImmutableArray<(int, double, double, int, int, int, int)> Areas { get; }

    public abstract void Build();

    public NormalType Normal(Point2D point) {
        if (point.PointType != PointType.Boundary) {
            throw new Exception("To determine the normal to the boundary, the point needs the boundary!");
        }

        var normalType = NormalType.None;

        foreach (var area in Areas) {
            normalType = point switch {
                _ when point.X == LinesX[area.Item4] => NormalType.LeftX,
                _ when point.Y == LinesY[area.Item6] => NormalType.BottomY,
                _ when point.X == LinesX[area.Item5] => NormalType.RightX,
                _ when point.Y == LinesY[area.Item7] => NormalType.UpperY,

                _ => NormalType.None,
            };

            if (normalType != NormalType.None) {
                break;
            }
        }

        return normalType;
    }

    protected PointType PointTypesWithoutInternalCheck(double x, double y) {
        foreach (var area in Areas) {
            if (x >= LinesX[area.Item4] && x <= LinesX[area.Item5] && y >= LinesY[area.Item6] && y <= LinesY[area.Item7]) {
                return PointType.Boundary;
            }
        }

        return PointType.Dummy;
    }

    protected void InternalCheck() {
        foreach (var point in Points.Where(point => point.PointType != PointType.Dummy)) {
            if (Points.Any(lstPoint => lstPoint.I == point.I - 1 && lstPoint.J == point.J && lstPoint.PointType != PointType.Dummy) &&
                Points.Any(lstPoint => lstPoint.I == point.I + 1 && lstPoint.J == point.J && lstPoint.PointType != PointType.Dummy) &&
                Points.Any(lstPoint => lstPoint.J == point.J - 1 && lstPoint.I == point.I && lstPoint.PointType != PointType.Dummy) &&
                Points.Any(lstPoint => lstPoint.J == point.J + 1 && lstPoint.I == point.I && lstPoint.PointType != PointType.Dummy)) {
                point.PointType = PointType.Internal;
            }
        }
    }

    protected void SetAreaNumber() {
        for (int i = 0; i < Points.Count; i++) {
            for (int iArea = 0; iArea < Areas.Length; iArea++) {
                if (Points[i].X >= Areas[iArea].Item4 && Points[i].X <= Areas[iArea].Item5 &&
                    Points[i].Y >= Areas[iArea].Item6 && Points[i].Y <= Areas[iArea].Item7) {
                    Points[i].AreaNumber = Areas[iArea].Item1;
                }
            }
        }
    }

    public void AssignBoundaryConditions(Boundary[] boundaries) {
        foreach (var point in Points.Where(point => point.PointType == PointType.Boundary)) {
            for (int k = 0; k < boundaries.Length; k++) {
                if (point.X >= LinesX[boundaries[k].X1] && point.X <= LinesX[boundaries[k].X2]
                && point.Y >= LinesY[boundaries[k].Y1] && point.Y <= LinesY[boundaries[k].Y2]) {
                    point.BoundaryType = boundaries[k].BoundaryType;
                    break;
                }
            }
        }
    }

    protected void WriteToFilePoints() {
        using (var sw = new StreamWriter("points/boundaryPoints.txt")) {
            Points.ForEach(x => { if (x.PointType == PointType.Boundary) sw.WriteLine(x); });
        }

        using (var sw = new StreamWriter("points/internalPoints.txt")) {
            Points.ForEach(x => { if (x.PointType == PointType.Internal) sw.WriteLine(x); });
        }

        using (var sw = new StreamWriter("points/dummyPoints.txt")) {
            Points.ForEach(x => { if (x.PointType == PointType.Dummy) sw.WriteLine(x); });
        }
    }
}