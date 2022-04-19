namespace eMP_2;

public class BandMatrix {
    public double[][] Upper { get; set; }
    public double[][] Lower { get; set; }
    public double[] Diagonal { get; set; }
    public int Size { get; init; }
    public int ColumnCount { get; init; }

    public BandMatrix(int size, int columnCount) {
        Size = size;
        ColumnCount = columnCount;
        Diagonal = new double[size];
        Upper = new double[size].Select(column => new double[columnCount]).ToArray();
        Lower = new double[size].Select(column => new double[columnCount]).ToArray();
    }

    public void Clear() {
        for (int i = 0; i < Size; i++) {
            Diagonal[i] = 0;

            for (int j = 0; j < ColumnCount; j++) {
                if (i + j < ColumnCount)
                    continue;

                Lower[i][j] = 0;
                Upper[i][j] = 0;
            }
        }
    }

    public static double[] operator *(BandMatrix matrix, double[] vector) {
        double[] result = new double[matrix.Size];

        for (int i = 0; i < matrix.Size; i++) {
            result[i] = matrix.Diagonal[i] * vector[i];

            for (int j = 0; j < matrix.ColumnCount; j++) {
                if (i + j < matrix.ColumnCount)
                    continue;

                int index = Math.Abs(matrix.ColumnCount - i - j);

                result[i] += matrix.Lower[i][j] * vector[index];
                result[index] += matrix.Upper[i][j] * vector[i];
            }
        }

        return result;
    }
}