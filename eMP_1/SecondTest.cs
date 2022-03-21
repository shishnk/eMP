namespace eMF_1;

public class SecondTest : ITest
{
    public double U(Point2D point)
        => point.X * point.X - point.Y;
        
    public double F(Point2D point)
        => -1 + point.X * point.X - point.Y;
}