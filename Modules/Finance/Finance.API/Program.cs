using System.Threading.RateLimiting;
using Finance.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH")
                  ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var financeLogPath = Path.Combine(logBasePath, "finance-api", "finance-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(financeLogPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(
        policyName: "ApiPolicy",
        options =>
        {
            options.PermitLimit = 100;
            options.Window = TimeSpan.FromMinutes(1);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 10;
        });

    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddFinanceInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.Map(
        "/error",
        static (HttpContext context) =>
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An internal server error has occurred.",
                Instance = context.Request.Path.Value
            };

            context.Response.ContentType = "application/problem+json";
            return Results.Json(problem, statusCode: StatusCodes.Status500InternalServerError);
        });
}

var port = Environment.GetEnvironmentVariable("FINANCE_API_PORT") ?? "5003";
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
