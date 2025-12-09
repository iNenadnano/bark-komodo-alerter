using System.Text.Json.Serialization;

namespace KomodoBarkAlerter.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeverityLevel
{
    Ok,
    Warning,
    Critical
}
