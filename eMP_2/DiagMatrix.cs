namespace eMP_2;

public class DiagMatrix {
    public double[][] Diags { get; set; }
    public int[] Indexes { get; init; }
    public int Size { get; init; }

    public DiagMatrix(int countPoints) {
        Size = countPoints;
        Diags = new double[5][];
        Diags[0] = new double[countPoints];
        Diags[1] = new double[countPoints - 1];
        Diags[2] = new double[countPoints - 2];
        Diags[3] = new double[countPoints - 1];
        Diags[4] = new double[countPoints - 2];
        Indexes = new int[] { 0, -1, -2, 1, 2 };
    }
}