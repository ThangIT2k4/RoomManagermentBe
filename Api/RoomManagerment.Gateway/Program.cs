using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
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

// Redis:Configuration (env Redis__Configuration) hoặc ConnectionStrings:Redis; Docker override = redis:6379
var redisConfig = builder.Configuration["Redis:Configuration"]
    ?? builder.Configuration.GetConnectionString("Redis")
    ?? "redis:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig;
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "RoomManagementGateway:";
});
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(_ =>
    StackExchange.Redis.ConnectionMultiplexer.Connect(redisConfig));
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
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

var redisConnection = builder.Configuration["Redis:Configuration"]
    ?? builder.Configuration.GetConnectionString("Redis")
    ?? "redis:6379";
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

builder.Services.AddOpenApiDocument();

var app = builder.Build();

#region ===== MIDDLEWARE PIPELINE =====

// Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseStatusCodePages();

// HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

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

// Error endpoint
if (!app.Environment.IsDevelopment())
{
    app.Map("/error", HandleException);
}

#endregion

// Read port from environment or launchSettings, with fallback
var port = Environment.GetEnvironmentVariable("GATEWAY_API_PORT") ?? "5000";
var protocol = app.Environment.IsDevelopment() ? "http" : "https";

try
{
    Log.Information("Gateway attempting to bind on {Protocol}://0.0.0.0:{Port}", protocol, port);
    app.Run($"{protocol}://0.0.0.0:{port}");
}
catch (IOException ex) when (ex.InnerException is SocketException)
{
    Log.Fatal(ex, "Failed to bind to port {Port}. Port may already be in use", port);
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
