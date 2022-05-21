namespace eMP_3;

public static class ArrayHelper {
    public static T[] Copy<T>(this T[] source, T[] destination) {
        for (int i = 0; i < source.Length; i++) {
            destination[i] = source[i];
        }

        return destination;
    }
}