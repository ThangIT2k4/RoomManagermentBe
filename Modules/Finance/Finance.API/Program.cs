using System.Data.Common;
using System.Diagnostics;
using System.Threading.RateLimiting;
using Finance.Infrastructure;
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

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH")
                  ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var financeLogPath = Path.Combine(logBasePath, "finance-api", "finance-api-.json");

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File(new JsonFormatter(), financeLogPath, rollingInterval: RollingInterval.Day));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

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

var wrappedFactoryType = InterceptorCore.Initialize("Finance.API", typeof(NpgsqlFactory));

DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

RuntimeConfiguration.ConfigureDQE<PostgreSqlDQEConfiguration>(c =>
{
    c.AddDbProviderFactory(wrappedFactoryType); // dùng provider Npgsql
    c.SetTraceLevel(TraceLevel.Verbose); // bật log (optional)
});

RuntimeConfiguration.Tracing
    .SetTraceLevel("ORMPersistenceExecution", TraceLevel.Verbose)
    .SetTraceLevel("ORMPlainSQLQueryExecution", TraceLevel.Verbose);

builder.Services.AddFinanceInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseRoomManagermentExceptionHandling();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseCors("ApiPolicy");
app.UseSerilogRequestLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

app.MapControllers();

// ASPNETCORE_URLS được set bởi Docker Compose → dùng nó; chỉ fallback khi chạy dotnet run trực tiếp
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS"))
    && string.IsNullOrEmpty(builder.Configuration["urls"]))
{
    var port = Environment.GetEnvironmentVariable("FINANCE_API_PORT")
               ?? builder.Configuration["FINANCE_API_PORT"]
               ?? "5203";
    app.Urls.Add($"http://0.0.0.0:{port}");
}
app.Run();
