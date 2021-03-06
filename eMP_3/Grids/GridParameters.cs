namespace eMP_3;

public class GridParametersJsonConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is null) {
            writer.WriteNull();
            return;
        }

        var gridParameters = (GridParameters)value;

        writer.WriteStartObject();
        writer.WritePropertyName("Initial area in X");
        serializer.Serialize(writer, gridParameters.IntervalX);
        writer.WritePropertyName("Splits by X");
        writer.WriteValue(gridParameters.SplitsX);
        writer.WriteWhitespace("\n");

        writer.WritePropertyName("Initial area in Y");
        serializer.Serialize(writer, gridParameters.IntervalY);
        writer.WritePropertyName("Splits by Y");
        writer.WriteValue(gridParameters.SplitsY);
        writer.WriteWhitespace("\n");

        writer.WritePropertyName("Initial area in Z");
        serializer.Serialize(writer, gridParameters.IntervalZ);
        writer.WritePropertyName("Splits by Z");
        writer.WriteValue(gridParameters.SplitsZ);
        writer.WriteWhitespace("\n");

        writer.WriteComment("Коэффициент разрядки");
        writer.WritePropertyName("Coef");
        writer.WriteValue(gridParameters.K);

        writer.WriteComment("Коэффициенты уравнения");
        writer.WritePropertyName("Lambda");
        writer.WriteValue(gridParameters.Lambda);
        writer.WritePropertyName("Omega");
        writer.WriteValue(gridParameters.Omega);
        writer.WritePropertyName("Chi");
        writer.WriteValue(gridParameters.Chi);
        writer.WriteEndObject();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType == JsonToken.Null || reader.TokenType != JsonToken.StartObject)
            return null;

        Interval intervalX;
        Interval intervalY;
        Interval intervalZ;
        int splitsX;
        int splitsY;
        int splitsZ;
        double? coef;
        double lambda, omega, sigma, chi;

        var maintoken = JObject.Load(reader);

        var token = maintoken["Initial area in X"];
        intervalX = serializer.Deserialize<Interval>(token!.CreateReader());
        token = maintoken["Splits by X"];
        splitsX = Convert.ToInt32(token);

        token = maintoken["Initial area in Y"];
        intervalY = serializer.Deserialize<Interval>(token!.CreateReader());
        token = maintoken["Splits by Y"];
        splitsY = Convert.ToInt32(token);

        token = maintoken["Initial area in Z"];
        intervalZ = serializer.Deserialize<Interval>(token!.CreateReader());
        token = maintoken["Splits by Z"];
        splitsZ = Convert.ToInt32(token);

        token = maintoken["Coef"];
        if (token is not null) {
            coef = double.TryParse(token.ToString(), out double res) ? res : null;
        } else {
            coef = null;
        }

        token = maintoken["Lambda"];
        lambda = Convert.ToDouble(token);

        token = maintoken["Omega"];
        omega = Convert.ToDouble(token);

        token = maintoken["Sigma"];
        sigma = Convert.ToDouble(token);

        token = maintoken["Chi"];
        chi = Convert.ToDouble(token);

        return new GridParameters(intervalX, splitsX, intervalY, splitsY, intervalZ, splitsZ, coef, lambda, omega, sigma, chi);
    }

    public override bool CanConvert(Type objectType)
        => objectType == typeof(GridParameters);
}

[JsonConverter(typeof(GridParametersJsonConverter))]
public readonly record struct GridParameters {
    public Interval IntervalX { get; init; }
    public int SplitsX { get; init; }
    public Interval IntervalY { get; init; }
    public int SplitsY { get; init; }
    public Interval IntervalZ { get; init; }
    public int SplitsZ { get; init; }
    public double? K { get; init; }
    public double Lambda { get; init; }
    public double Omega { get; init; }
    public double Sigma { get; init; }
    public double Chi { get; init; }

    public GridParameters(Interval intervalX, int splitsX, Interval intervalY, int splitsY, Interval intervalZ, int splitsZ,
                         double? k, double lambda, double omega, double sigma, double chi) {
        IntervalX = intervalX;
        SplitsX = splitsX;
        IntervalY = intervalY;
        SplitsY = splitsY;
        IntervalZ = intervalZ;
        SplitsZ = splitsZ;
        K = k;
        Lambda = lambda;
        Omega = omega;
        Sigma = sigma;
        Chi = chi;
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
            return null;
        }
    }
}