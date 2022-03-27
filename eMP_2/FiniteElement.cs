namespace eMP_2;

public class FiniteElement {
    public Interval Interval { get; init; }

    public FiniteElement(Interval interval)
        => Interval = interval;

    public override string ToString()
        => $"Element interval is [{Interval.LeftBorder}; {Interval.RightBorder}]";
}