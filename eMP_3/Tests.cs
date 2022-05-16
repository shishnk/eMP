namespace eMP_3;

public interface ITest {
    public double Us(Point3D point);

    public double Fs(Point3D point);

    public double Uc(Point3D point);

    public double Fc(Point3D point);
}

public class Test1 : ITest {
    public double Fc(Point3D point)
        => -point.X - point.Y - point.Z;

    public double Fs(Point3D point)
        => (2 * point.X) + (2 * point.Y) + point.Z;

    public double Uc(Point3D point)
        => point.X + point.Y + point.Z;

    public double Us(Point3D point)
        => (2 * point.X) + (2 * point.Y) + point.Z;
}