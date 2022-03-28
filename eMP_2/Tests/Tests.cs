namespace eMP_2;

public interface ITest {
    public double U(double point);

    public double F(double point);
}

public class FirstTest : ITest {
    public double U(double point)
        => point;

    public double F(double point)
        => 0;
}