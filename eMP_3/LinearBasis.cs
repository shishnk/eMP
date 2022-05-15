namespace eMP_3;

public static class LinearBasis {
    public static double Psi1(double x, double y, double z)
        => (1 - x) * (1 - y) * (1 - z);

    public static double Psi2(double x, double y, double z)
        => x * (1 - y) * (1 - z);

    public static double Psi3(double x, double y, double z)
        => y * (1 - x) * (1 - z);

    public static double Psi4(double x, double y, double z)
        => z * (1 - x) * (1 - y);

    public static double Psi5(double x, double y, double z)
        => (1 - x) * y * z;

    public static double Psi6(double x, double y, double z)
        => x * y * (1 - z);

    public static double Psi7(double x, double y, double z)
        => x * z * (1 - y);

    public static double Psi8(double x, double y, double z)
        => x * z * y;
}