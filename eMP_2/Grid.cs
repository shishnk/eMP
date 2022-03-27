namespace eMP_2;

public abstract class Grid {
    public abstract ImmutableArray<double>? Points { get; }

    protected abstract void Build(GridParameters gridParameters);
}