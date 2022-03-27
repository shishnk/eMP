namespace eMP_2;

public readonly record struct FiniteElement {
    public Interval Interval { get; init; }

    public FiniteElement(Interval interval)
        => Interval = interval;

    public override string ToString()
        => $"Element interval is [{Interval.LeftBorder}; {Interval.RightBorder}]";
}