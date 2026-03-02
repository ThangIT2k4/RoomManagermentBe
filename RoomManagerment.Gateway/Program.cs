using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING =====
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

// ===== REQUEST SIZE LIMIT =====
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
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

// ===== RATE LIMITER =====
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
        
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5
        });
    });
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ===== PROBLEM DETAILS =====
builder.Services.AddProblemDetails();

// ===== REVERSE PROXY =====
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOpenApiDocument();

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====

// 1. Exception Handling (FIRST)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStatusCodePages();

// 2. HSTS + HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

// 3. Forwarded Headers (CRITICAL - BEFORE Rate Limiting)
app.UseForwardedHeaders();

// 4. Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// 5. CORS
app.UseCors("GatewayPolicy");

// 6. Rate Limiting (CRITICAL - BEFORE Reverse Proxy)
app.UseRateLimiter();

// 7. Development endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi();
}

// 8. Reverse Proxy
app.MapReverseProxy();

// 9. Error handler
if (!app.Environment.IsDevelopment())
{
    app.Map("/error", HandleException);
}

app.Run();

// ===== ERROR HANDLER =====
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