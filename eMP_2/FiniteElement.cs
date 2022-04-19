namespace eMP_2;

public readonly record struct FiniteElement(Interval Interval) {
    public override string ToString()
        => $"Element interval is [{Interval.LeftBorder}; {Interval.RightBorder}]";
}