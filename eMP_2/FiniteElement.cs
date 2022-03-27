namespace eMP_2;

public readonly record struct Element {
    public double LeftBorder { get; init; }
    public double RightBorder { get; init; }
    public double Lenght { get; init; }

    public Element(double leftBorder, double rightBorder) {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        Lenght = Math.Abs(rightBorder - leftBorder);
    }

    public override string ToString()
        => $"Element interval is [{LeftBorder}; {RightBorder}]";
}