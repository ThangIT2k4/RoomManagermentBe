using RoomManagerment.ExternalApi.Consumers;
using RoomManagerment.Messaging.Extensions;
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
builder.Services.AddControllers();


builder.Services.AddRabbitMqMessaging(builder.Configuration, x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

var urls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
if (urls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true)
{
    app.UseHttpsRedirection();
}

app.MapControllers();

Log.Information("External API ASPNETCORE_URLS: {Urls}", urls ?? "(Kestrel defaults)");

try
{
    app.Run();
}
catch (IOException ex) when (ex.InnerException is SocketException)
{
    Log.Fatal(ex, "Failed to bind. ASPNETCORE_URLS={Urls}", urls);
    throw;
}