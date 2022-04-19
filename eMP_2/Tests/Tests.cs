namespace eMP_2;

public interface ITest {
    public double U(double point);

    public double F(double point);
}

public class Test1 : ITest {
    public double U(double point)
        => point * point;

    public double F(double point)
        => -5 * point * point;
}