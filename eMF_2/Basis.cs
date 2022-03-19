namespace eMF_2;

/// <summary>
/// $x = \xi = \dfrac{x(точка) - x_{2k-1}} / h_k$,
/// где $h_k = x_{2k+1} - x_{2k-1}$ (длина конечного элемента) 
/// </summary>
public static class Basis {
    public static double Psi1(double x)
        => 2 * (x - 0.5) * (x - 1);

    public static double Psi2(double x)
        => -4 * x * (x - 1);

    public static double Psi3(double x)
        => 2 * x * (x - 0.5);
}