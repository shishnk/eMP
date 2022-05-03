namespace eMP_2;

public record Integration {
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

    public double GaussOrder5(MFE.Basis psiI, MFE.Basis psiJ, double a, double b) {
        double qi, pi;
        double result = 0.0;

        double h = b - a;

        for (int i = 0; i < 3; i++) {
            qi = _weights[i];
            pi = (a + b + _points[i] * h) / 2.0;

            result += qi * psiI(pi) * psiJ(pi);
        }

        return result * h / 2.0;
    }

    public double GaussOrder5(Func<double, double> lambda, MFE.Basis psiI, MFE.Basis psiJ, double a, double b) {
        double qi, pi;
        double result = 0.0;

        double h = b - a;

        for (int i = 0; i < 3; i++) {
            qi = _weights[i];
            pi = (a + b + _points[i] * h) / 2.0;

            result += qi * lambda(pi) * psiI(pi) * psiJ(pi);
        }

        return result * h / 2.0;
    }
}