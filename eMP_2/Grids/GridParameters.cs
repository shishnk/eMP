namespace eMP_2;

public class GridParameters {
    [JsonProperty("Initial area")]
    public Interval Interval { get; init; }

    [JsonProperty("Number of splits")]
    public int Splits { get; init; }

    [JsonProperty("Coef")]
    public double? K { get; init; } // коэффициент разрядки

    public GridParameters(Interval interval, int splits, double? k) {
        Interval = interval;
        Splits = splits;
        K = k;
    }
}