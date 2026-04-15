using Identity.Application;
using Identity.API.Requests;
using Identity.API.Security;
using Identity.API.Validators;
using Identity.Infrastructure;
using RoomManagerment.Messaging.Extensions;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using System.Net.Sockets;
using IOException = System.IO.IOException;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING =====
var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var identityLogPath = Path.Combine(logBasePath, "identity-api", "identity-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(identityLogPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

// ===== SERVICES =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Session store = Redis (đăng ký trong Identity.Infrastructure) để share session với Gateway
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services
    .AddAuthentication(SessionAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
        SessionAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization();

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
                             ?? builder.Configuration["CORS_ALLOWED_ORIGINS"]?
                                 .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
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
    
    // Login endpoint: 5 attempts per minute
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "LoginPolicy", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
    
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ===== PROBLEM DETAILS =====
builder.Services.AddProblemDetails();

// ===== APPLICATION & INFRASTRUCTURE =====
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// ===== RABBITMQ MESSAGING =====
// Identity chỉ publish events (không cần consumer ở đây)
builder.Services.AddRabbitMqMessaging(builder.Configuration);

var app = builder.Build();

var listenUrls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
var listensHttps = listenUrls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true;

// ===== MIDDLEWARE PIPELINE =====

// 1. Exception Handling (FIRST)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStatusCodePages();

// 2. HSTS + HTTPS (only when Kestrel actually listens on HTTPS)
if (!app.Environment.IsDevelopment() && listensHttps)
{
    app.UseHsts();
}
if (listensHttps)
    app.UseHttpsRedirection();

// 3. Forwarded Headers (BEFORE Rate Limiting)
app.UseForwardedHeaders();

// 4. Request Logging
app.UseSerilogRequestLogging();

// 5. CORS
app.UseCors("ApiPolicy");

// 6. Session (must run before session-based authentication)
app.UseSession();

// 7. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 8. Rate Limiting
app.UseRateLimiter();


// 9. Development endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

// 10. Map Controllers
app.MapControllers();

// 11. Error handler
if (!app.Environment.IsDevelopment())
{
    app.Map("/error", HandleException);
}

try
{
    app.Run();
}
catch (IOException ex) when (ex.InnerException is SocketException)
{
    Log.Fatal(ex, "Failed to bind. ASPNETCORE_URLS={Urls}", listenUrls);
    throw;
}

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

