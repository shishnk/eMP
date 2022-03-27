namespace eMP_2;

public abstract class NestedGrid : IGrid {
    private double[]? _points;
    public abstract bool TimeDependent { get; }
    public ImmutableArray<double>? Points => _points?.ToImmutableArray();

    protected NestedGrid(GridParameters gridParameters)
        => Build(gridParameters);

    private void Build(GridParameters gridParameters) {
        try {
            if (TimeDependent == true && gridParameters.Interval.LeftBorder < 0)
                throw new Exception("The beginning of the time segment cannot be less than 0");

            ArgumentNullException.ThrowIfNull(gridParameters.K, $"{nameof(gridParameters.K)} cannot be null");

            _points = new double[gridParameters.Splits + 1];
            double h;
            double sum = 0;
            double coef = Math.Sqrt((double)gridParameters.K);

            for (int k = 0; k < gridParameters.Splits; k++)
                sum += Math.Pow(coef, k);

            h = gridParameters.Interval.Lenght / sum;
            _points[0] = gridParameters.Interval.LeftBorder;
            _points[^1] = gridParameters.Interval.RightBorder;

            int index = 1;

            while (_points[index] != gridParameters.Interval.RightBorder) {
                _points[index] = _points[index - 1] + h;
                h *= (double)gridParameters.K;
                index++;
            }

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }
}