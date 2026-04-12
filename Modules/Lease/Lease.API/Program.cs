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

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference("/swagger");
}

app.MapControllers();

var port = Environment.GetEnvironmentVariable("LEASE_API_PORT") ?? "5207";
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
