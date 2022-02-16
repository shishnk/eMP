namespace eMF_1;

public class FirstTest : ITest
{
    public double U(Point2D point)
        => point.X * point.X;

    public double F(Point2D point)
        => -4;
}