namespace eMP_3;

public class RegularGrid : Grid {
    private readonly Point3D[] _points = default!;
    public override ImmutableArray<Point3D> Points => _points.ToImmutableArray();

    public RegularGrid(GridParameters gridParameters) {
        _points = new Point3D[(gridParameters.SplitsX + 1) * (gridParameters.SplitsY + 1) * (gridParameters.SplitsZ + 1)];
        Build(gridParameters);
    }

    private void Build(GridParameters gridParameters) {
        try {
            if (gridParameters.SplitsX < 1 || gridParameters.SplitsY < 1 || gridParameters.SplitsZ < 1)
                throw new Exception("The number of splits must be greater than or equal to 1");

            double hx = gridParameters.IntervalX.Lenght / gridParameters.SplitsX;
            double hy = gridParameters.IntervalY.Lenght / gridParameters.SplitsY;
            double hz = gridParameters.IntervalZ.Lenght / gridParameters.SplitsZ;

            double[] pointsX = new double[gridParameters.SplitsX + 1];
            double[] pointsY = new double[gridParameters.SplitsY + 1];
            double[] pointsZ = new double[gridParameters.SplitsZ + 1];

            pointsX[0] = gridParameters.IntervalX.LeftBorder;
            pointsY[0] = gridParameters.IntervalY.LeftBorder;
            pointsZ[0] = gridParameters.IntervalZ.LeftBorder;

            for (int i = 1; i < pointsX.Length; i++) {
                pointsX[i] = pointsX[i - 1] + hx;
            }

            for (int i = 1; i < pointsY.Length; i++) {
                pointsY[i] = pointsY[i - 1] + hy;
            }

            for (int i = 1; i < pointsY.Length; i++) {
                pointsZ[i] = pointsZ[i - 1] + hz;
            }

            int index = 0;

            for (int i = 0; i < pointsZ.Length; i++) {
                for (int j = 0; j < pointsY.Length; j++) {
                    for (int k = 0; k < pointsX.Length; k++) {
                        _points[index++] = new(pointsX[k], pointsY[j], pointsZ[i]);
                    }
                }
            }

            WritePoints();

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    private void WritePoints() {
        var sw = new StreamWriter("points.txt");
        using (sw) {
            for (int i = 0; i < _points.Length; i++) {
                sw.WriteLine(_points[i]);
            }
        }
    }
}