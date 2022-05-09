namespace eMP_3;

public abstract class Grid {
    public abstract ImmutableArray<Point3D> Points { get; }
    public abstract ImmutableArray<ImmutableArray<int>> Elements { get; }
    public abstract ImmutableArray<ImmutableArray<int>> Sides { get; }
}