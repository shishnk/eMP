namespace eMP_2;

public class GridParameters {
    [JsonProperty("Initial area")]
    public Interval Interval { get; init; }

    [JsonProperty("Number of splits")]
    public int Splits { get; init; }

    [JsonProperty("Coef")]
    public double? K { get; init; } // коэффициент разрядки
    public double? Sigma { get; init; }

    public GridParameters(Interval interval, int splits, double? k, double? sigma) {
        Interval = interval;
        Splits = splits;
        K = k;
        Sigma = sigma;
    }

    public static GridParameters? ReadJson(string jsonPath) {
        try {
            if (!File.Exists(jsonPath))
                throw new Exception("File does not exist");

            var sr = new StreamReader(jsonPath);
            using (sr) {
                return JsonConvert.DeserializeObject<GridParameters>(sr.ReadToEnd());
            }

        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
        }

        return null;
    }
}