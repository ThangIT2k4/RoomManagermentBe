using Scalar.AspNetCore;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                             ?? builder.Configuration["CORS_ALLOWED_ORIGINS"]?
                                 .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             ?? new[] { "http://localhost:3000", "http://localhost:4200" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


builder.Services.AddRabbitMqMessaging(builder.Configuration, x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

app.UseSerilogRequestLogging();

var urls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
if (!app.Environment.IsDevelopment()
    && urls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true)
{
    app.UseHttpsRedirection();
}

app.UseCors("ApiPolicy");

app.MapControllers();

Log.Information("External API ASPNETCORE_URLS: {Urls}", urls ?? "(Kestrel defaults)");

try
{
    app.Run();
}
catch (IOException ex) when (ex.InnerException is SocketException se)
{
    if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
    {
        Log.Fatal(
            ex,
            "Port already in use (duplicate External API or compound run?). ss -tlnp | grep 5204. ASPNETCORE_URLS={Urls}",
            urls);
    }
    else
    {
        Log.Fatal(ex, "Failed to bind. ASPNETCORE_URLS={Urls}", urls);
    }

    throw;
}