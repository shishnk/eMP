namespace eMP_2;

public class IntervalConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is null) {
            writer.WriteNull();
            return;
        }

        Interval interval = (Interval)value;

        writer.WriteStartObject();
        writer.WritePropertyName("Left border");
        writer.WriteValue(interval.LeftBorder);
        writer.WritePropertyName("Right border");
        writer.WriteValue(interval.RightBorder);
        writer.WriteEndObject();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType == JsonToken.Null || reader.TokenType != JsonToken.StartObject)
            return null;

        Interval value;

        var maintoken = JObject.Load(reader);

        var token = maintoken["Left border"];
        double leftBorder = Convert.ToDouble(token);

        token = maintoken["Right border"];
        double rightBorder = Convert.ToDouble(token);

        value = new(leftBorder, rightBorder);

        return value;
    }

    public override bool CanConvert(Type objectType)
        => objectType == typeof(Interval);
}

[JsonConverter(typeof(IntervalConverter))]
public readonly record struct Interval {
    public double LeftBorder { get; init; }
    public double RightBorder { get; init; }
    public double Center { get; }
    public double Lenght { get; }

    public Interval(double leftBorder, double rightBorder) {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        Center = (leftBorder + rightBorder) / 2;
        Lenght = rightBorder - leftBorder;
    }
}