namespace eMF_1;

public interface ITest
{
    public double U(Point2D point);
    public double F(Point2D point);
}

public class FirstTest : ITest
{
    public double U(Point2D point)
        => point.X;

    public double F(Point2D point)
        => 0;
}

public class SecondTest : ITest
{
    public double U(Point2D point)
        => point.X * point.X - point.Y;
        
    public double F(Point2D point)
        => -1 + point.X * point.X - point.Y;
}

public class ThirdTest : ITest
{
    public double U(Point2D point)
        => 3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y;

    public double F(Point2D point)
        => (point.AreaNumber == 0) ? -9 * point.X - 6 * point.Y + 0.5 *
        (3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y) :
        -36 * point.X - 24 * point.Y + 2 * (3 * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y);
}

public class FourthTest : ITest
{
    public double U(Point2D point)
        => Math.Log(point.X + point.Y);

    public double F(Point2D point)
        => 2 / ((point.X + point.Y) * (point.X + point.Y));
}

public class FifthTest : ITest
{
    public double U(Point2D point)
        => 4 * point.X * point.X * point.X * point.X;
    public double F(Point2D point)
        => -48 * point.X * point.X;
}

public class SixthTest : ITest
{
    public double U(Point2D point)
        => 4 * point.X * point.X * point.X * point.X + 2 * point.Y * point.Y * point.Y * point.Y;

    public double F(Point2D point)
        => -48 * point.X * point.X - 24 * point.Y * point.Y;
}

public class SeventhTest : ITest
{
    public double U(Point2D point)
        => Math.Exp(point.X) + point.Y;

    public double F(Point2D point)
        => point.Y;
}

public class EighthTest : ITest
{
    public double U(Point2D point)
        => point.X * point.X * point.X + point.Y;

    public double F(Point2D point)
        => -6 * point.X;
}