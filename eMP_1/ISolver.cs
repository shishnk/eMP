namespace eMF_1;

public interface ISolver
{
    public int MaxIters { get; init; }
    public double Eps { get; init; }
    public double W { get; init; }

    public double[] Compute(DiagMatrix diagMatrix, double[] pr);
}