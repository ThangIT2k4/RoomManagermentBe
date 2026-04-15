using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Gateway.Models;
using RoomManagerment.Messaging.Extensions;
using RoomManagerment.Shared.Http;
using Serilog;
using StackExchange.Redis;
using System.Net.Sockets;
using IOException = System.IO.IOException;

var builder = WebApplication.CreateBuilder(args);


#region LOGGING

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var gatewayLogPath = Path.Combine(logBasePath, "gateway", "gateway-.txt");

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProcessId()
        .WriteTo.Console()
        .WriteTo.File(gatewayLogPath, rollingInterval: Serilog.RollingInterval.Day);
});

#endregion

#region REDIS

// Cùng chuỗi ưu tiên với các API (Redis__Configuration, REDIS_CONFIGURATION, REDIS_HOST+PORT+PASSWORD, …)
var redisConfig = RedisServiceExtensions.ResolveConnectionString(builder.Configuration);
var redisOptions = ConfigurationOptions.Parse(redisConfig);
redisOptions.AbortOnConnectFail = false;
redisOptions.ConnectRetry = 3;
redisOptions.ConnectTimeout = 5000;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = redisOptions;
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "RoomManagementGateway:";
});
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(_ =>
    StackExchange.Redis.ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddHostedService<RoomManagerment.Gateway.Services.NotificationPushRedisSubscriber>();

#endregion

#region SESSION

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(
        builder.Configuration.GetValue<int>("Session:TimeoutMinutes", 30)
    );

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "RoomManager.SessionId";
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

#endregion

#region REQUEST SIZE LIMIT

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 
        builder.Configuration.GetValue<long>("Kestrel:MaxRequestBodySize", 10 * 1024 * 1024); // Default to 10MB
});

#endregion

#region FORWARDED HEADER

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayPolicy", policy =>
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
    
#endregion

#region RATE LIMITER

builder.Services.AddRateLimiter(options =>
{
    // 1️⃣ Global concurrency limit (protect CPU)
    options.AddConcurrencyLimiter("concurrency", opt =>
    {
        opt.PermitLimit = 100; // max 100 request running at same time
        opt.QueueLimit = 50;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // 2️⃣ IP + User based rate limit
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : null;

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var partitionKey = userId ?? clientIp;

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, // 100 req
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 5,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

#endregion

#region PROBLEM DETAILS

builder.Services.AddProblemDetails();

#endregion

#region SIGNALR

var redisConnection = RedisServiceExtensions.ResolveConnectionString(builder.Configuration);
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnection, options =>
    {
        options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("RoomManagerment:SignalR:");
    });

#endregion

#region REVERSE PROXY

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<RoomManagerment.Gateway.Transforms.SessionUserIdTransformProvider>();

builder.Services.AddOpenApiDocument();

#endregion

var app = builder.Build();

var listenUrls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
var listensHttps = listenUrls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true;

#region ===== MIDDLEWARE PIPELINE =====

// Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseStatusCodePages();

// HTTPS (only when Kestrel listens on HTTPS — Docker typically uses http:// in ASPNETCORE_URLS)
if (!app.Environment.IsDevelopment() && listensHttps)
{
    app.UseHsts();
}

if (listensHttps)
    app.UseHttpsRedirection();

// Forwarded headers
app.UseForwardedHeaders();

// Logging
app.UseSerilogRequestLogging();

// CORS
app.UseCors("GatewayPolicy");

// Session (phải trước MapHub để Hub đọc được session)
app.UseSession();

// Rate limiting
app.UseRateLimiter();

// SignalR Hub (notification realtime; client connect với cookie session)
app.MapHub<RoomManagerment.Gateway.Hubs.NotificationsHub>("/hubs/notifications");

// Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi();
}

// Reverse proxy
app.MapReverseProxy();

app.MapGet("/api/gateway/health", () =>
    Results.Json(ApiResponse<GatewayHealthPayload>.Succeed(new GatewayHealthPayload())));

// Error endpoint
if (!app.Environment.IsDevelopment())
{
    app.Map("/error", HandleException);
}

#endregion

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
            "Port already in use (duplicate Gateway or compound run?). ss -tlnp | grep 5200. ASPNETCORE_URLS={Urls}",
            listenUrls);
    }
    else
    {
        Log.Fatal(ex, "Failed to bind. ASPNETCORE_URLS={Urls}", listenUrls);
    }

    throw;
}

#region HELPER METHODS

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

    return Results.Json(problem,
        statusCode: StatusCodes.Status500InternalServerError,
        contentType: "application/problem+json");
}

#endregion
