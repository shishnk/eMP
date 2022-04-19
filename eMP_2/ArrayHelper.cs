namespace eMP_2;

public static class ArrayHelper {
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

    public static double[] Sub(this double[] fst, double[] snd) {
        double[] result = new double[fst.Length];

        for (int i = 0; i < fst.Length; i++)
            result[i] = fst[i] - snd[i];

        return result;
    }
}