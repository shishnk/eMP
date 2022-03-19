namespace eMF_2;
[Serializable]
class InvalidTapeMatrixLinesCount : Exception {
    public InvalidTapeMatrixLinesCount() {  }
}

class InvalidMatrixIndeces : Exception {
    public InvalidMatrixIndeces() {  }
}

interface IMatrix { 
    public double ByIndex(uint i, uint j);
}

/// <summary>
/// Класс <c>TapeMatrix</c> используется для создания матрицы
/// с ленточным форматом хранения элементов.
/// </summary>
public class TapeMatrix : IMatrix {
    public double[][] RawData { get; private set; }
    public uint Dimension { get; init; }
    public uint Lines { get; init; }

    /// <summary>
    /// Конструктор в качестве аргументов принимает размерности матрицы <c>dimension</c> и
    /// количество лент <c>lines</c>, которое должно быть нечётным числом.
    /// Матрица хранится по строкам.
    /// </summary>
    public TapeMatrix(uint dimension, uint lines) {
        if (lines % 2 == 0) {
            throw new InvalidTapeMatrixLinesCount();
        }

        RawData = new double[dimension][];
        for(uint i = 0; i < lines; i++) {
            RawData[i] = new double[lines];
        }

        Dimension = dimension;
        Lines = lines;
    }

    /// <summary>
    /// Метод <c>ByIndex</c> возвращает элемент матрицы по индексам как если бы она была плотной.
    /// Первый аргумент <c>i</c> - строка, второй <c>j</c> - столбец, индексация с нуля.
    /// </summary>
    public double ByIndex(uint i, uint j) {
        if (i > Dimension || j > Dimension) {
            throw new InvalidMatrixIndeces();
        }

        if (Math.Abs(i - j) > Lines / 2) {
            return 0.0;
        }

        return RawData[i][j - i + 1];
    }
}