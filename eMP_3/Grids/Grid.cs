namespace eMP_3;

public abstract class Grid {
    public abstract ImmutableArray<Point3D> Points { get; }
    public abstract ImmutableList<Point3D> InternalPoints { get; }
}