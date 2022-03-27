namespace eMP_2;

public interface IGrid {
    public bool TimeDependent { get; }
    public ImmutableArray<double>? Points { get; }
}