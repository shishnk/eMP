namespace eMF_1;

public class EighthTest : ITest
{
    public double U(Point2D point)
        => point.X * point.X * point.X + point.Y;

    public double F(Point2D point)
        => -6 * point.X;
}