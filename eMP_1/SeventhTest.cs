namespace eMF_1;

public class SeventhTest : ITest
{
    public double U(Point2D point)
        => Math.Exp(point.X) + point.Y;

    public double F(Point2D point)
        => point.Y;
}