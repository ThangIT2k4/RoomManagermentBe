using System.Data.Common;
using System.Diagnostics;
using Lease.Infrastructure;
using Scalar.AspNetCore;
using Npgsql;
using SD.LLBLGen.Pro.DQE.PostgreSql;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.Tools.OrmProfiler.Interceptor;

var builder = WebApplication.CreateBuilder(args);

var wrappedFactoryType = InterceptorCore.Initialize("Lease.API", typeof(NpgsqlFactory));

DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

RuntimeConfiguration.ConfigureDQE<PostgreSqlDQEConfiguration>(c =>
{
    c.AddDbProviderFactory(wrappedFactoryType); // dùng provider Npgsql
    c.SetTraceLevel(TraceLevel.Verbose); // bật log (optional)
});

RuntimeConfiguration.Tracing
    .SetTraceLevel("ORMPersistenceExecution", TraceLevel.Verbose)
    .SetTraceLevel("ORMPlainSQLQueryExecution", TraceLevel.Verbose);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("ApiPolicy");

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference("/swagger");
}

app.MapControllers();

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS"))
    && string.IsNullOrEmpty(builder.Configuration["urls"]))
{
    var port = Environment.GetEnvironmentVariable("LEASE_API_PORT")
               ?? builder.Configuration["LEASE_API_PORT"]
               ?? "5207";
    app.Urls.Add($"http://0.0.0.0:{port}");
}
app.Run();
