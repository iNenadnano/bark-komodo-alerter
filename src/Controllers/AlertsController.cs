using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using BarkKomodoAlerter.Model;
using Microsoft.AspNetCore.Mvc;

namespace BarkKomodoAlerter.Controllers;

[ApiController]
[Route("alert")]
public class AlertsController(IHttpClientFactory httpClientFactory, ILogger<AlertsController> logger) : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<AlertsController> _logger = logger;

    private static readonly JsonSerializerOptions JsonOptions;

    static AlertsController()
    {
        JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        JsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    [HttpPost]
    public async Task<IActionResult> PostAlert([FromBody] JsonElement alertElement)
    {
        _logger.LogInformation("Received alert payload: {AlertPayload}", alertElement.GetRawText());

        Alert? alert;
        try
        {
            alert = alertElement.Deserialize<Alert>(JsonOptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "invalid alert payload", details = ex.Message });
        }

        if (alert is null)
        {
            return BadRequest(new { error = "alert payload was empty or invalid" });
        }

        var keys = ReadDeviceKeys();

        if (keys.Length == 0)
        {
            return BadRequest(new { error = "No device keys configured. Set environment variable BARK_DEVICE_KEYS (comma-separated)." });
        }

        var barkEndpoint = Environment.GetEnvironmentVariable("BARK_ENDPOINT") ?? "https://api.day.app";
        var barkGroup = Environment.GetEnvironmentVariable("BARK_GROUP");
        var barkIcon = Environment.GetEnvironmentVariable("BARK_ICON");
        var barkPrefix = Environment.GetEnvironmentVariable("BARK_TITLE_PREFIX");
        var barkSound = Environment.GetEnvironmentVariable("BARK_ALERT_SOUND");
        barkEndpoint += "/push";
        
        var payload = AlertFormatter.CreateBarkPayload(alert, keys, barkPrefix, barkGroup, barkIcon, barkSound);

        var client = _httpClientFactory.CreateClient("bark");
        var response = await client.PostAsJsonAsync(barkEndpoint, payload);
        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            var body = new
            {
                error = $"Bark responded with {(int)response.StatusCode} {response.ReasonPhrase}",
                details = text
            };
            return StatusCode((int)HttpStatusCode.BadGateway, body);
        }

        return Ok();
    }

    private static string[] ReadDeviceKeys()
    {
        var raw = Environment.GetEnvironmentVariable("BARK_DEVICE_KEYS");
        if (string.IsNullOrWhiteSpace(raw))
        {
            return [];
        }

        return [.. raw
            .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))];
    }
}
