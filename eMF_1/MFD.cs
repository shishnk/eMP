namespace eMF_1;

public enum PointType
{
    Boundary,
    Internal,
    Dummy
}

public enum BoundaryType
{
    None,
    Dirichlet,
    Neumann,
    Mixed
}

public enum GridType
{
    Regular,
    Irregular
}

public class MFD
{
    private Grid _grid;
    private ITest _test;
    private ISolver _solver;

    public MFD(Grid grid)
    {
        _grid = grid;
    }

    public void SetTest(ITest test)
        => _test = test;

    public void SetMethodSolvingSLAE(ISolver solver)
        => _solver = solver;

    public void Compute()
    {
        try
        {
            if (_test is null)
                throw new Exception("Set the test!");

            if (_solver is null)
                throw new Exception("Set the method solving SLAE!");

            _grid.Build();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}