namespace eMP_2;

public class Integration {
    private readonly double[] _points;
    private readonly double[] _weights;

    public Integration() {
        _points = new double[3];
        _weights = new double[3];

        _points[0] = 0.0;
        _points[1] = Math.Sqrt(3.0 / 5.0);
        _points[2] = -Math.Sqrt(3.0 / 5.0);

        _weights[0] = 8.0 / 9.0;
        _weights[1] = 5.0 / 9.0;
        _weights[2] = 5.0 / 9.0;
    }

    public double GaussOrder5(MFE.Basis PsiI, MFE.Basis PsiJ) {
        double qi, pi;
        double result = 0;

        for (int i = 0; i < 3; i++) {
            qi = _weights[i];
            pi = (1 + _points[i]) / 2.0;

            result += qi * PsiI(pi) * PsiJ(pi);
        }

        return result / 2.0;
    }
}