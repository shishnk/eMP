namespace eMF_2;

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
            throw "Число диагоналей должно быть нечётным.";
        }

        RawData = new double[dimensions][lines];
        Dimension = dimension;
        Lines = lines;
    }

    /// <summary>
    /// Метод <c>ByIndex</c> возвращает элемент матрицы по индексам как если бы она была плотной.
    /// Первый аргумент <c>i</c> - строка, второй <c>j</c> - столбец, индексация с нуля.
    /// </summary>
    public double ByIndex(uint i, uint j) {
        if (i > Dimension || j > Dimension) {
            throw "У матрицы нет элемента с такими индексами.";
        }

        if (Abs(i - j) > Lines / 2) {
            return 0.0;
        }

        return RawData[i][j - i + 1];
    }
}