using Organization.Infrastructure;
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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("ORGANIZATION_API_PORT") ?? "5014";
var protocol = app.Environment.IsDevelopment() ? "http" : "https";
app.Run($"{protocol}://0.0.0.0:{port}");
