namespace eMP_3;

public abstract class Grid {
    public abstract ImmutableArray<Point3D> Points { get; }
    public abstract ImmutableArray<ImmutableArray<int>> Elements { get; }
    public abstract ImmutableArray<ImmutableArray<int>> Sides { get; }
    public double Lambda { get; init; }
    public double Omega { get; init; }
    public double Sigma { get; init; }
    public double Chi { get; init; }

    protected Grid(GridParameters gridParameters)
        => (Lambda, Omega, Sigma, Chi) = (gridParameters.Lambda, gridParameters.Omega, gridParameters.Sigma, gridParameters.Chi);
}