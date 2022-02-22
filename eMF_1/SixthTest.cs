namespace eMF_1;

public class SixthTest : ITest
{
    public double U(Point2D point)
        => 4 * point.X * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y * point.Y;

    public double F(Point2D point)
        => -48 * point.X * point.X - 24 * point.Y * point.Y;
}