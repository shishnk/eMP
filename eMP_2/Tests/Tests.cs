namespace eMP_2;

public interface ITest {
    public double U(double x, double t);

    public double F(double x, double t);

    public Func<double, double> Lambda(Func<double, double> lambda);

    public double DerivativeLambda(double weight);
}

public class Test1 : ITest {
    public double U(double x, double t)
        => x + t;

    public double F(double x, double t)
        => 0;

    public Func<double, double> Lambda(Func<double, double> lambda)
        => (u) => lambda(u);

    public double DerivativeLambda(double weight)
        => 1;
}

public class Test2 : ITest {
    public double U(double x, double t)
        => x * x + t;

    public double F(double x, double t)
        => -6 * x * x - 2 * t + 1;

    public Func<double, double> Lambda(Func<double, double> lambda)
        => (u) => lambda(u);

    public double DerivativeLambda(double weight)
        => 1;
}

public class Test3 : ITest {
    public double U(double x, double t)
        => 2 * x * x * x * t;

    public double F(double x, double t)
        => -192 * x * x * x * x * x * x * x * t * t * t + 4 * x * x * x;

    public Func<double, double> Lambda(Func<double, double> lambda)
        => (u) => lambda(u) * lambda(u);

    public double DerivativeLambda(double weight)
        => 2 * weight;
}

public class Test4 : ITest {
    public double U(double x, double t)
        => Math.Exp(x) * t;

    public double F(double x, double t)
        => -t * Math.Exp(x + t * Math.Exp(x)) - t * t * Math.Exp(2 * x + t * Math.Exp(x)) + 0.5 * Math.Exp(x);

    public Func<double, double> Lambda(Func<double, double> lambda)
        => (u) => Math.Exp(lambda(u));

    public double DerivativeLambda(double weight)
        => Math.Exp(weight);
}