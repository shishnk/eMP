namespace eMP_2;

public abstract class Decomposer {
    protected DiagMatrix _matrix = default!;
    protected double[] _vector = default!;
    protected double[]? _solution;
    public ImmutableArray<double>? Solution => _solution?.ToImmutableArray();

    public void SetMatrix(DiagMatrix matrix)
        => _matrix = matrix;

    public void SetVector(double[] vector)
        => _vector = vector;

    public abstract void Compute();
}

public class DecomposerLU : Decomposer {
    public override void Compute() {
        return;
    }
}