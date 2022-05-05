namespace eMP_3;

public readonly record struct Interval {
    [JsonProperty("Left Border")]
    public double LeftBorder { get; init; }

    [JsonProperty("Right Border")]
    public double RightBorder { get; init; }

    [JsonIgnore]
    public double Lenght { get; init; }

    [JsonConstructor]
    public Interval(double leftBorder, double rightBorder) {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        Lenght = Math.Abs(rightBorder - leftBorder);
    }
}