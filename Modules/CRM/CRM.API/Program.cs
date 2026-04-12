using System.Threading.RateLimiting;
using CRM.API.Validators;
using CRM.Application.Features.Leads;
using CRM.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Shared.Extensions;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var crmLogPath = Path.Combine(logBasePath, "crm-api", "crm-api-.json");

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File(new JsonFormatter(), crmLogPath, rollingInterval: RollingInterval.Day));

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

var listenUrls =
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
    ?? builder.Configuration["urls"];
var listensHttps = listenUrls?.Contains("https://", StringComparison.OrdinalIgnoreCase) == true;

app.UseRoomManagermentExceptionHandling();

if (!app.Environment.IsDevelopment() && listensHttps)
{
    app.UseHsts();
}

app.UseStatusCodePages();
if (listensHttps)
{
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

app.MapControllers();

app.Run();
