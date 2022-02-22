namespace eMF_1;

public class FifthTest : ITest
{
    public double U(Point2D point)
        => 4 * point.X * point.X * point.X * point.X;
    public double F(Point2D point)
        => -48 * point.X * point.X;
}