using System.Text.Json.Serialization;

namespace BarkKomodoAlerter.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeverityLevel
{
    Ok,
    Warning,
    Critical
}
