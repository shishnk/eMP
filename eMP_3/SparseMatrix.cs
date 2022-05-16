namespace eMP_3;

public class SparseMatrix {
    // public fields - its bad, but the readability is better
    public int[] ig = default!;
    public int[] jg = default!;
    public double[] di = default!;
    public double[] ggl = default!;
    public double[] ggu = default!;
    public int Size { get; init; }

    public SparseMatrix(int size, int sizeOffDiag) {
        Size = size;
        ig = new int[size + 1];
        jg = new int[sizeOffDiag];
        ggl = new double[sizeOffDiag];
        ggu = new double[sizeOffDiag];
        di = new double[size];
    }

    public static Vector<double> operator *(SparseMatrix matrix, Vector<double> vector) {
        Vector<double> product = new(vector.Length);

        for (int i = 0; i < vector.Length; i++) {
            product[i] = matrix.di[i] * vector[i];

            for (int j = matrix.ig[i]; j < matrix.ig[i + 1]; j++) {
                product[i] += matrix.ggl[j] * vector[matrix.jg[j]];
                product[matrix.jg[j]] += matrix.ggu[j] * vector[i];
            }
        }

        return product;
    }

    public void PrintDense(string path) {
        double[,] A = new double[Size, Size];

        for (int i = 0; i < Size; i++) {
            A[i, i] = di[i];

            for (int j = ig[i]; j < ig[i + 1]; j++) {
                A[i, jg[j]] = ggl[j];
                A[jg[j], i] = ggu[j];
            }
        }

        using var sw = new StreamWriter(path);
        for (int i = 0; i < Size; i++) {
            for (int j = 0; j < Size; j++) {
                sw.Write(A[i, j].ToString("0.0000") + "\t");
            }

            sw.WriteLine();
        }
    }
}