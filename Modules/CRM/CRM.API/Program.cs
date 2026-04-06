using System.Threading.RateLimiting;
using CRM.API.Validators;
using CRM.Application.Features.Leads;
using CRM.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var crmLogPath = Path.Combine(logBasePath, "crm-api", "crm-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(crmLogPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IValidator<CreateLeadRequest>, CreateLeadRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateLeadStatusRequest>, UpdateLeadStatusRequestValidator>();

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

builder.Services.AddInfrastructure(builder.Configuration);

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
    app.Map("/error", static (HttpContext context) =>
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
    });
}

var port = Environment.GetEnvironmentVariable("CRM_API_PORT") ?? "5008";
var protocol = app.Environment.IsDevelopment() ? "http" : "https";
app.Run($"{protocol}://0.0.0.0:{port}");
