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
app.UseHttpsRedirection();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("EXTERNAL_API_PORT") ?? "5004";
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