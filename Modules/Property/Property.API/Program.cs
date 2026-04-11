using Property.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

var port = Environment.GetEnvironmentVariable("PROPERTY_API_PORT") ?? "5206";
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
