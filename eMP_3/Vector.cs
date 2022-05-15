namespace eMP_3;

public class Vector<T> where T : INumber<T> {
    private readonly T[] vec;
    public int Length { get; init; }

    public T this[int index] {
        get => vec[index];
        set => vec[index] = value;
    }

    public Vector(int dim) {
        vec = new T[dim];
        Length = dim;
    }

    public static T operator *(Vector<T> firstVec, Vector<T> secondVec) {
        T result = T.Zero;

        for (int i = 0; i < firstVec.Length; i++) {
            result += firstVec.vec[i] * secondVec.vec[i];
        }

        return result;
    }

    public static Vector<T> operator *(double constant, Vector<T> vector) {
        Vector<T> result = new(vector.Length);

        for (int i = 0; i < vector.Length; i++) {
            result.vec[i] = vector.vec[i] * T.Create(constant);
        }

        return result;
    }

    public static Vector<T> operator +(Vector<T> firstVec, Vector<T> secondVec) {
        Vector<T> result = new(firstVec.Length);

        for (int i = 0; i < firstVec.Length; i++) {
            result.vec[i] = firstVec.vec[i] + secondVec.vec[i];
        }

        return result;
    }

    public static Vector<T> operator -(Vector<T> firstVec, Vector<T> secondVec) {
        Vector<T> result = new(firstVec.Length);

        for (int i = 0; i < firstVec.Length; i++) {
            result.vec[i] = firstVec.vec[i] - secondVec.vec[i];
        }

        return result;
    }

    public static void Copy(Vector<T> source, Vector<T> destination) {
        for (int i = 0; i < source.Length; i++) {
            destination[i] = source[i];
        }
    }

    public void Fill(double value) {
        for (int i = 0; i < Length; i++) {
            vec[i] = T.Create(value);
        }
    }

    public ImmutableArray<T> ToImmutableArray()
        => ImmutableArray.Create(vec);
}