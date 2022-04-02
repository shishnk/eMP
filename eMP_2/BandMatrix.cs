namespace eMP_2;

public class BandMatrix {
    public double[][] Upper { get; set; }
    public double[][] Lower { get; set; }
    public double[] Diagonal { get; set; }
    public int Size { get; init; }

    public BandMatrix(int size, int columnCount) {
        Size = size;
        Diagonal = new double[size];
        Upper = new double[size].Select(column => new double[columnCount]).ToArray();
        Lower = new double[size].Select(column => new double[columnCount]).ToArray();
    }
}