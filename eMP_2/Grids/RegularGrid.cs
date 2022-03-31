namespace eMP_2;

public abstract class RegularGrid : IGrid {
    private double[] _points = default!;
    public double? Sigma { get; init; }
    public abstract bool TimeDependent { get; }
    public ImmutableArray<double> Points => _points.ToImmutableArray();

    protected RegularGrid(GridParameters gridParameters) {
        Sigma = gridParameters.Sigma;
        Build(gridParameters);
    }

    private void Build(GridParameters gridParameters) {
        if (TimeDependent == true && gridParameters.Interval.LeftBorder < 0)
            throw new Exception("The beginning of the time segment cannot be less than 0");

        if (gridParameters.Splits < 1)
            throw new Exception("The number of splits must be greater than or equal to 1");

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