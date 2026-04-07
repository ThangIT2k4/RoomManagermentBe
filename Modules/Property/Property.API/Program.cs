using Property.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PROPERTY_API_PORT") ?? "5007";
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
