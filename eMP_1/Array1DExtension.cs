namespace eMP_1;

public static class Array1DExtension {
    public static double Norm(this double[] array) {
        double result = 0;

        for (int i = 0; i < array.Length; i++)
            result += array[i] * array[i];

        return Math.Sqrt(result);
    }

    public static void Fill(this double[] array, double value) {
        for (int i = 0; i < array.Length; i++)
            array[i] = value;
    }

    public static void Copy(this double[] source, double[] destination) {
        for (int i = 0; i < source.Length; i++)
            destination[i] = source[i];
    }
}