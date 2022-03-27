namespace eMP_2;

public class SpaceRegularGrid : Grid {
    private double[]? _points;
    public override ImmutableArray<double>? Points => _points?.ToImmutableArray();

     public SpaceRegularGrid(GridParameters gridParameters) {
        Build(gridParameters);
    }

    protected override void Build(GridParameters gridParameters) {
        _points = new double[gridParameters.Splits + 1];
        double h = gridParameters.Interval.Lenght / gridParameters.Splits;

        _points[0] = gridParameters.Interval.LeftBorder;
        _points[^1] = gridParameters.Interval.RightBorder;
        int index = 1;

        while (_points[index] != gridParameters.Interval.RightBorder) {
            _points[index] = _points[index - 1] + h;
            index++;
        }
    }
}