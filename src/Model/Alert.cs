using System.Text.Json.Serialization;
using System.Text.Json;

namespace BarkKomodoAlerter.Model
{
    public record Alert(
    [property: JsonPropertyName("_id")] JsonElement? Id,
    [property: JsonPropertyName("ts")] long? Ts,
    [property: JsonPropertyName("resolved")] bool Resolved,
    [property: JsonPropertyName("level")] SeverityLevel Level,
    [property: JsonPropertyName("target")] JsonElement? Target,
    [property: JsonPropertyName("data")] AlertData Data,
    [property: JsonPropertyName("resolved_ts")] long? ResolvedTs);
}