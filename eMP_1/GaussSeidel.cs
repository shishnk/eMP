namespace eMF_1;

public record GaussSeidel(int MaxIters, double Eps, double W) : ISolver
{
    public double[] Compute(DiagMatrix diagMatrix, double[] pr)
    {
        double[] qk = new double[diagMatrix.Size];
        double[] qk1 = new double[diagMatrix.Size];
        double[] residual = new double[diagMatrix.Size];
        double prNorm = pr.Norm();

        for (int i = 0; i < MaxIters; i++)
        {
            for (int k = 0; k < diagMatrix.Size; k++)
            {
                double fstSum = MultLine(diagMatrix, k, qk1, 1);
                double scdSum = MultLine(diagMatrix, k, qk, 2);

                residual[k] = pr[k] - (fstSum + scdSum);
                qk1[k] = qk[k] + W * residual[k] / diagMatrix.Diags[0][k];
            }

            qk1.Copy(qk);
            qk1.Fill(0);

            if (residual.Norm() / prNorm < Eps)
                break;
        }

        return qk;
    }

    private double MultLine(DiagMatrix diagMatrix, int i, double[] vector, int method)
    {
        double sum = 0;

        if (method == 0 || method == 1)
        {
            if (i > 0)
            {
                sum += diagMatrix.Diags[1][i - 1] * vector[i - 1];

                if (i > diagMatrix.ZeroDiags + 1)
                    sum += diagMatrix.Diags[2][i - diagMatrix.ZeroDiags - 2] * vector[i - diagMatrix.ZeroDiags - 2];
            }
        }

        if (method == 0 || method == 2)
        {
            sum += diagMatrix.Diags[0][i] * vector[i];

            if (i < diagMatrix.Size - 1)
            {
                sum += diagMatrix.Diags[3][i] * vector[i + 1];

                if (i < diagMatrix.Size - diagMatrix.ZeroDiags - 2)
                    sum += diagMatrix.Diags[4][i] * vector[i + diagMatrix.ZeroDiags + 2];
            }
        }

        return sum;
    }
}