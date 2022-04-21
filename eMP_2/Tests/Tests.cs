namespace eMP_2;

public interface ITest {
    public double U(double point, double t);

    public double F(double point, double t);
}

public class Test1 : ITest {
    public double U(double point, double t)
        => point * point + t;

    public double F(double point, double t)
        => -6 * point * point - 2 * t + 1;
}