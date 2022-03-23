namespace eMP_2;

public record struct Element {
    public double LeftBorder { get; init; }
    public double RightBorder { get; init; }
    public double Lenght { get; init; }

    public Element(double leftBorder, double rightBorder) {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        Lenght = Math.Abs(rightBorder - leftBorder);
    }

    public static Element Parse(string str) {
        var data = str.Split();
        Element element = new(double.Parse(data[0]), double.Parse(data[1]));

        return element;
    }

    public override string ToString()
        => $"Element interval is [{LeftBorder}; {RightBorder}]";
}