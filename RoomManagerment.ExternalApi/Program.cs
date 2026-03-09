using Serilog;
using System.Net.Sockets;
using IOException = System.IO.IOException;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var externalLogPath = Path.Combine(logBasePath, "external-api", "external-api-.txt");
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console()
        .WriteTo.File(externalLogPath, rollingInterval: Serilog.RollingInterval.Day)
        .MinimumLevel.Information());

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

// Read port from environment or launchSettings, with fallback
var port = Environment.GetEnvironmentVariable("EXTERNAL_API_PORT") ?? "5001";
var protocol = app.Environment.IsDevelopment() ? "http" : "https";

try
{
    Log.Information("External API attempting to bind on {Protocol}://0.0.0.0:{Port}", protocol, port);
    app.Run($"{protocol}://0.0.0.0:{port}");
}
catch (IOException ex) when (ex.InnerException is SocketException)
{
    Log.Fatal(ex, "Failed to bind to port {Port}. Port may already be in use", port);
    throw;
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}