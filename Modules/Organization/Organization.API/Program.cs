using Organization.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var logPath = Path.Combine(logBasePath, "organization-api", "organization-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var listenUrls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
var listensHttps = listenUrls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

if (listensHttps)
    app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();
