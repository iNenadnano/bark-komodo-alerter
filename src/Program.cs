using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Keep console quieter by ignoring Microsoft.* info logs (e.g., endpoint execution)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Logging.AddFilter("BarkKomodoAlerter", LogLevel.Warning);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpClient("bark", client =>
{
    // Client base address is set per-request because device keys may change.
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.MapControllers();

app.Run();
