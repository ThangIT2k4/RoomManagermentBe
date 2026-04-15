using System.Data.Common;
using System.Diagnostics;
using Organization.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using Npgsql;
using SD.LLBLGen.Pro.DQE.PostgreSql;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.Tools.OrmProfiler.Interceptor;

var builder = WebApplication.CreateBuilder(args);

var logBasePath = Environment.GetEnvironmentVariable("LOG_BASE_PATH") ?? "/home/thang/projects/WorkSpace/Projects/RoomManagerment/Logs";
var logPath = Path.Combine(logBasePath, "organization-api", "organization-api-.txt");
builder.Services.AddSerilog(
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger());

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

var wrappedFactoryType = InterceptorCore.Initialize("Organization.API", typeof(NpgsqlFactory));

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/swagger");
}

if (listensHttps)
    app.UseHttpsRedirection();

app.UseCors("ApiPolicy");

app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();
