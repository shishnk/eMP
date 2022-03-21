namespace eMF_1;

public class FourthTest : ITest
{
    public double U(Point2D point)
        => Math.Log(point.X + point.Y);

    public double F(Point2D point)
        => 2 / ((point.X + point.Y) * (point.X + point.Y));
}