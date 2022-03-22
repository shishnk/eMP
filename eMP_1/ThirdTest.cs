namespace eMF_1;

public class ThirdTest : ITest
{
    public double U(Point2D point)
        => 3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y;

    public double F(Point2D point)
        => (point.AreaNumber == 0) ? -9 * point.X - 6 * point.Y + 0.5 *
        (3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y) :
        -36 * point.X - 24 * point.Y + 2 * (3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y);
}