namespace eMF_1;

public interface ISolver
{
    public int MaxIters { get; init; }
    public double Eps { get; init; }

    public void Compute();
}