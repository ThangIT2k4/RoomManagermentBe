using System.Data.Common;
using System.Diagnostics;
using System.Threading.RateLimiting;
using CRM.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Shared.Extensions;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Formatting.Json;
using Npgsql;
using SD.LLBLGen.Pro.DQE.PostgreSql;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.Tools.OrmProfiler.Interceptor;

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

var wrappedFactoryType = InterceptorCore.Initialize("CRM.API", typeof(NpgsqlFactory));

DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

RuntimeConfiguration.ConfigureDQE<PostgreSqlDQEConfiguration>(c =>
{
    c.AddDbProviderFactory(wrappedFactoryType); // dùng provider Npgsql
    c.SetTraceLevel(TraceLevel.Verbose); // bật log (optional)
});

RuntimeConfiguration.Tracing
    .SetTraceLevel("ORMPersistenceExecution", TraceLevel.Verbose)
    .SetTraceLevel("ORMPlainSQLQueryExecution", TraceLevel.Verbose);

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
