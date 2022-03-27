namespace eMP_2;

public class TimeIrregularGrid : Grid {
    private double[]? _points;
    public override ImmutableArray<double>? Points => _points?.ToImmutableArray(); 

     public TimeIrregularGrid(GridParameters gridParameters) {
        Build(gridParameters);
    }

    protected override void Build(GridParameters gridParameters) {
     try {
            ArgumentNullException.ThrowIfNull(gridParameters.K, $"{nameof(gridParameters.K)} cannot be null");

            _points = new double[gridParameters.Splits + 1];
            double h;
            double sum = 0;

            for (int k = 0; k < gridParameters.Splits; k++)
                sum += Math.Pow((double)gridParameters.K, k);

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