using System.Text.Json.Serialization;
using KomodoBarkAlerter.Model;

var builder = WebApplication.CreateBuilder(args);

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
