using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Notification.Application;
using Notification.API.Common;
using Notification.Infrastructure;
using Notification.Infrastructure.Consumers;
using RoomManagerment.Messaging.Extensions;
using Serilog;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using IOException = System.IO.IOException;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING =====
var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var notificationLogPath = Path.Combine(logBasePath, "notification-api", "notification-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(notificationLogPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

// ===== SERVICES =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// ===== REQUEST SIZE LIMIT =====
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5 * 1024 * 1024; // 5MB
});

// ===== FORWARDED HEADERS =====
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000", "http://localhost:4200" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ===== RATE LIMITING =====
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "ApiPolicy", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ===== APPLICATION & INFRASTRUCTURE =====
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ===== RABBITMQ MESSAGING =====
// Notification API là consumer: lắng nghe events từ các service khác
builder.Services.AddRabbitMqMessaging(builder.Configuration, x =>
{
    x.AddConsumer<NotificationCreateRequestedConsumer>();
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<PasswordChangedConsumer>();
    x.AddConsumer<UserLoggedInConsumer>();
    x.AddConsumer<SessionCreatedConsumer>();
    x.AddConsumer<SessionRevokedConsumer>();
    x.AddConsumer<UserEmailVerifiedConsumer>();
});

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStatusCodePages();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseForwardedHeaders();
app.UseSerilogRequestLogging();
app.UseCors("ApiPolicy");
app.UseRateLimiter();

var internalSharedKey = builder.Configuration["InternalAuth:SharedKey"];
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    if (string.IsNullOrWhiteSpace(internalSharedKey))
    {
        await next();
        return;
    }

    var providedKey = context.Request.Headers[SessionUserIdHeader.InternalServiceKeyName].FirstOrDefault();
    if (!string.Equals(providedKey, internalSharedKey, StringComparison.Ordinal))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = "Invalid internal service key." });
        return;
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.Map("/error", HandleException);
}

var port = Environment.GetEnvironmentVariable("NOTIFICATION_API_PORT") ?? "5001";
var protocol = app.Environment.IsDevelopment() ? "http" : "https";

try
{
    Log.Information("Notification API attempting to bind on {Protocol}://0.0.0.0:{Port}", protocol, port);
    app.Run($"{protocol}://0.0.0.0:{port}");
}
catch (IOException ex) when (ex.InnerException is SocketException)
{
    Log.Fatal(ex, "Failed to bind to port {Port}. Port may already be in use", port);
    throw;
}

static IResult HandleException(HttpContext context)
{
    var problem = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        title = "Internal Server Error",
        status = StatusCodes.Status500InternalServerError,
        detail = "An internal server error has occurred.",
        instance = context.Request.Path
    };

    context.Response.ContentType = "application/problem+json";
    return Results.Json(problem, statusCode: StatusCodes.Status500InternalServerError);
}
