namespace eMP_2;

public interface IGrid {
    public bool TimeDependent { get; }
    public double? Sigma { get; init; }
    public ImmutableArray<double> Points { get; }
}