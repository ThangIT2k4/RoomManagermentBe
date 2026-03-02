using Identity.Application;
using Identity.Infrastructure;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING =====
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/identity-api-.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

// ===== SERVICES =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDistributedMemoryCache();

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

// ===== SESSION =====
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".RoomManager.Session";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ===== APPLICATION & INFRASTRUCTURE =====
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// 3. Forwarded Headers (BEFORE Rate Limiting)
app.UseForwardedHeaders();

// 4. Request Logging
app.UseSerilogRequestLogging();

// 5. CORS
app.UseCors("ApiPolicy");

// 6. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Rate Limiting
app.UseRateLimiter();

// 8. Session
app.UseSession();

// 9. Development endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 10. Map Controllers
app.MapControllers();

// 11. Error handler
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

