namespace eMP_2;

public class TimeRegularGrid : Grid {
    private double[]? _points;
    public override ImmutableArray<double>? Points => _points?.ToImmutableArray();

    public TimeRegularGrid(GridParameters gridParameters) {
        Build(gridParameters);
    }

    protected override void Build(GridParameters gridParameters) {
        try {
            if (gridParameters.Interval.LeftBorder < 0)
                throw new Exception("The beginning of the time segment cannot be less than 0");

            _points = new double[gridParameters.Splits + 1];
            double h = gridParameters.Interval.Lenght / gridParameters.Splits;

            _points[0] = gridParameters.Interval.LeftBorder;
            _points[^1] = gridParameters.Interval.RightBorder;
            int index = 1;

            while (_points[index] != gridParameters.Interval.RightBorder) {
                _points[index] = _points[index - 1] + h;
                index++;
            }

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }
}