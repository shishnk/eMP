namespace eMP_2;

public abstract class Decomposer {
    protected BandMatrix _matrix = default!;
    protected double[] _vector = default!;
    protected double[]? _solution;
    public ImmutableArray<double>? Solution => _solution?.ToImmutableArray();

    public void SetMatrix(BandMatrix matrix)
        => _matrix = matrix;

    public void SetVector(double[] vector)
        => _vector = vector;

    public abstract void Compute();
}

public class DecomposerLU : Decomposer {
    public override void Compute() {
        // Ax = LUx = b; Ly = b, Ux = y;
        _solution = new double[_matrix.Size];
        _vector.Copy(_solution);
        LU();
        ForwardElimination();
        BackSubstitution();
    }

    private void LU() {
        // We walk through diagonal elements
        for (int idx = 0; idx < _matrix.Size; idx++) {
            // Then we walk through row and column (except not-in-matrix elements)
            for (int offset = 0; offset < _matrix.ColumnCount; offset++) {
                if (idx + offset < _matrix.ColumnCount)
                    continue;

                double sumL = 0.0;
                double sumU = 0.0;

                int idx2 = idx - _matrix.ColumnCount + offset;

                // sum compution
                for (int ik = 0; ik < offset; ik++) {
                    int kj = _matrix.ColumnCount - offset + ik;

                    sumL += _matrix.Lower[idx][ik] * _matrix.Upper[idx2][kj];
                    sumU += _matrix.Upper[idx][ik] * _matrix.Lower[idx2][kj];
                }

                _matrix.Lower[idx][offset] -= sumL;
                _matrix.Upper[idx][offset] = (_matrix.Upper[idx][offset] - sumU) / _matrix.Diagonal[idx2];
            }

            double sumD = 0.0;

            // And then we change our diagonal
            for (int offset = 0; offset < _matrix.ColumnCount; offset++)
                sumD += _matrix.Lower[idx][offset] * _matrix.Upper[idx][offset];

            _matrix.Diagonal[idx] -= sumD;
        }
    }

    private void ForwardElimination() {
        for (int i = 0; i < _matrix.Size; i++) {
            double sumY = 0.0;

            for (int j = 0; j < _matrix.ColumnCount; j++) {
                if (i + j < _matrix.ColumnCount)
                    continue;

                sumY += _matrix.Lower[i][j] * _solution![i - _matrix.ColumnCount + j];
            }

            _solution![i] = (_solution[i] - sumY) / _matrix.Diagonal[i];
        }
    }

    private void BackSubstitution() {
        for (int i = _matrix.Size - 1; i >= 0; i--) {
            for (int j = _matrix.ColumnCount - 1; j >= 0; j--) {
                if (i + j < _matrix.ColumnCount)
                    continue;

                int idx = i - _matrix.ColumnCount + j;
                _solution![idx] -= _matrix.Upper[i][j] * _solution[i];
            }
        }
    }
}