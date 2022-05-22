namespace eMP_3;

public interface ITest {
    public double Us(Point3D point);

    public double Fs(Point3D point);

    public double Uc(Point3D point);

    public double Fc(Point3D point);
}

public class Test1 : ITest { // все коэффициенты равны 1
    public double Fc(Point3D point)
       => -point.X + point.Y;

    public double Fs(Point3D point)
        => (-3 * point.X) - (3 * point.Y) - (2 * point.Z);

    public double Us(Point3D point)
        => point.X + (2 * point.Y) + point.Z;

    public double Uc(Point3D point)
        => (2 * point.X) + point.Y + point.Z;
}