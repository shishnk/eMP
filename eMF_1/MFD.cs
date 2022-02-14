namespace eMF_1;

public enum PointType
{
    Boundary,
    Internal,
    Dummy
}

public enum BoundaryType
{
    Dirichlet = 1,
    Neumann = 2,
    Mixed = 3,
    None = 4
}

public class MFD
{
    Grid grid;

    public MFD(string areasPath, string boundariesPath)
    {
        grid = new(areasPath, boundariesPath);
    }

    public void Compute()
    {
        grid.Build();
    }
}