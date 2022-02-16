namespace eMF_1;

public class DiagMatrix
{
    public double[][] Diags { get; set; }
    public int Size { get; init; }
    public int ZeroDiags { get; init; }

    public DiagMatrix(int countPoints, int zeroDiags)
    {
        Size = countPoints;
        ZeroDiags = zeroDiags;
        Diags = new double[5][];
        Diags[0] = new double[countPoints];
        Diags[1] = new double[countPoints - 1];
        Diags[2] = new double[countPoints - zeroDiags - 2];
        Diags[3] = new double[countPoints - 1];
        Diags[4] = new double[countPoints - zeroDiags - 2];
    }
}