namespace eMP_3;

public enum SolverPreconditionings {
    None,
    LU
}

public abstract class Solver {
    protected SparseMatrix _matrix = default!;
    protected Vector<double> _vector = default!;
    protected Vector<double>? _solution;
    public int MaxIters { get; init; }
    public double Eps { get; init; }
    public SolverPreconditionings SolverPreconditioning { get; init; }
    public ImmutableArray<double>? Solution => _solution?.ToImmutableArray();

    protected Solver(int maxIters, double eps, SolverPreconditionings solverPreconditioning = SolverPreconditionings.None)
        => (MaxIters, Eps, SolverPreconditioning) = (maxIters, eps, solverPreconditioning);

    public void SetMatrix(SparseMatrix matrix)
        => _matrix = matrix;

    public void SetVector(Vector<double> vector)
        => _vector = vector;

    public abstract void Compute();
}

public class LOS : Solver {
    public LOS(int maxIters, double eps) : base(maxIters, eps) { }

    public override void Compute() {
        try {
            ArgumentNullException.ThrowIfNull(_matrix, $"{nameof(_matrix)} cannot be null, set the matrix");
            ArgumentNullException.ThrowIfNull(_vector, $"{nameof(_vector)} cannot be null, set the vector");

            // if (SolverPreconditioning == SolverPreconditionings.LU) {
            //     LOSWithLU();
            // }

            double alpha, beta;
            double squareNorm;

            _solution = new(_vector.Length);

            Vector<double> r = new(_vector.Length);
            Vector<double> z = new(_vector.Length);
            Vector<double> p = new(_vector.Length);
            Vector<double> tmp = new(_vector.Length);

            r = _vector - (_matrix * _solution);

            Vector<double>.Copy(r, z);

            p = _matrix * z;

            squareNorm = r * r;

            for (int index = 0; index < MaxIters && squareNorm > Eps; index++) {
                alpha = p * r / (p * p);
                _solution += alpha * z;
                squareNorm = (r * r) - (alpha * alpha * (p * p));
                r -= alpha * p;

                tmp = _matrix * r;

                beta = -(p * tmp) / (p * p);
                z = r + (beta * z);
                p = tmp + (beta * p);
            }
        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }

    // public void LOSWithLU() {
    //     uint index;
    //     double alpha, beta;
    //     double squareNorm;

    //     _solution = new(_vector.Length);

    //     Vector<double> r = new(_vector.Length);
    //     Vector<double> z = new(_vector.Length);
    //     Vector<double> p = new(_vector.Length);
    //     Vector<double> tmp = new(_vector.Length);

    //     LU();

    //     r = Direct(pr - MultDi(x));
    //     z = Reverse(r);
    //     p = Direct(Mult(z));

    //     squareNorm = r * r;

    //     for (index = 0; index < maxIter && squareNorm > eps; index++) {
    //         alpha = (p * r) / (p * p);
    //         squareNorm = (r * r) - (alpha * alpha) * (p * p);
    //         x = x + alpha * z;
    //         r = r - alpha * p;

    //         tmp = Direct(Mult(Reverse(r)));

    //         beta = -(p * tmp) / (p * p);
    //         z = Reverse(r) + beta * z;
    //         p = tmp + beta * p;
    //     }

    //     countIter = index;
    // }

    // private void LU() {
    //     double suml = 0.0;
    //     double sumu = 0.0;
    //     double sumdi = 0.0;

    //     for (uint i = 0; i < _matrix.Size; i++) {
    //         uint i0 = _matrix.ig[i];
    //         uint i1 = _matrix.ig[i + 1];

    //         for (uint k = i0; k < i1; k++) {
    //             uint j = _matrix.jg[k];
    //             uint j0 = _matrix.ig[j];
    //             uint j1 = _matrix.ig[j + 1];
    //             uint ik = i0;
    //             uint kj = j0;

    //             while (ik < k && kj < j1) {
    //                 if (jg[ik] == jg[kj]) {
    //                     suml += gglnew.vec[ik] * ggunew.vec[kj];
    //                     sumu += ggunew.vec[ik] * gglnew.vec[kj];
    //                     ik++;
    //                     kj++;
    //                 } else {
    //                     if (jg[ik] > jg[kj])
    //                         kj++;
    //                     else
    //                         ik++;
    //                 }
    //             }

    //             gglnew.vec[k] -= suml;
    //             ggunew.vec[k] = (ggunew.vec[k] - sumu) / dinew.vec[j];
    //             sumdi += gglnew.vec[k] * ggunew.vec[k];
    //             suml = 0;
    //             sumu = 0;
    //         }

    //         dinew.vec[i] -= sumdi;
    //         sumdi = 0;
    //     }
    // }
}