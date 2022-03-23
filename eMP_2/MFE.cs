namespace eMP_2;

public class MFE
{
    private ITest _test;
    private ISolver _solver;

    public MFE() { }


    public void Compute()
    {
        try
        {
            if (_test is null)
                throw new Exception("Set the test!");

            if (_solver is null)
                throw new Exception("Set the method of solving SLAE!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void SetTest(ITest test)
        => _test = test;

    public void SetMethodSolvingSLAE(ISolver solver)
        => _solver = solver;
}