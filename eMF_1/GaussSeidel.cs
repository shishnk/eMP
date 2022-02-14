namespace eMF_1;

public class GaussSeidel : ISolver
{
    public int MaxIters { get; init; }
    public double Eps { get; init; }

    public GaussSeidel(int maxIters, double eps)
    {
        MaxIters = maxIters;
        Eps = eps;
    }

    public void Compute() // TODO
    {
        throw new NotImplementedException();
    }
}