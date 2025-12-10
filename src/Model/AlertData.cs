using System.Text.Json.Serialization;
using System.Text.Json;

namespace BarkKomodoAlerter.Model
{
    public record AlertData(
    [property: JsonPropertyName("type")] AlertType Type,
    [property: JsonPropertyName("data")] JsonElement Data)
    {
        public string? GetString(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var current = Data;
            foreach (var part in parts)
            {
                if (!current.TryGetProperty(part, out var next)) return null;
                current = next;
            }
            return current.ValueKind == JsonValueKind.String ? current.GetString() : current.ToString();
        }

        public double GetDouble(string propertyName) =>
            Data.TryGetProperty(propertyName, out var el) && el.TryGetDouble(out var v) ? v : 0;

        public string GetVersion()
        {
            if (!Data.TryGetProperty("version", out var v)) return string.Empty;
            var major = v.TryGetProperty("major", out var ma) ? ma.GetInt32() : 0;
            var minor = v.TryGetProperty("minor", out var mi) ? mi.GetInt32() : 0;
            var patch = v.TryGetProperty("patch", out var pa) ? pa.GetInt32() : 0;
            return $"{major}.{minor}.{patch}";
        }

        public IEnumerable<string> GetStringArray(string propertyName)
        {
            if (!Data.TryGetProperty(propertyName, out var el) || el.ValueKind != JsonValueKind.Array) return Array.Empty<string>();
            return el.EnumerateArray().Select(e => e.ValueKind == JsonValueKind.String ? e.GetString() ?? string.Empty : e.ToString());
        }
    }
}
