namespace eMP_3;

public readonly record struct DirichletBoundary(int Element, int Side) {
    public static DirichletBoundary[]? ReadJson(string jsonPath) {
        try {
            if (!File.Exists(jsonPath))
                throw new Exception("File does not exist");

            var sr = new StreamReader(jsonPath);
            using (sr) {
                return JsonConvert.DeserializeObject<DirichletBoundary[]>(sr.ReadToEnd());
            }
        } catch (Exception ex) {
            Console.WriteLine($"We had problem: {ex.Message}");
            return null;
        }
    }
}
