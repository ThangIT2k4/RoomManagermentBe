using Auth.API.Controllers;
using Auth.Application;
using Auth.Application.Services;
using Auth.Infrastructure;
using Auth.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoomManagerment.Shared.Messaging;

namespace Auth.API.Tests.Integration;

internal sealed class AuthApiTestHost : IAsyncDisposable
{
    private readonly WebApplication _app;

    public AuthApiTestHost()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });
        builder.WebHost.UseTestServer();

        AuthServiceMock = new Mock<IAuthApplicationService>(MockBehavior.Strict);

        builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services
            .AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        builder.Services.AddAuthorization();

        builder.Services.AddValidatorsFromAssembly(typeof(MediatorAssemblyMarker).Assembly);
        builder.Services.AddAuthApplicationRequestHandlers();
        builder.Services.AddScoped<IAppSender, AppRequestSender>();
        builder.Services.AddScoped<IAuthApplicationService>(_ => AuthServiceMock.Object);

        _app = builder.Build();
        _app.UseRouting();
        _app.UseSession();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapControllers();
        _app.StartAsync().GetAwaiter().GetResult();

        Client = _app.GetTestClient();
    }

    public HttpClient Client { get; }
    public Mock<IAuthApplicationService> AuthServiceMock { get; }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return _app.DisposeAsync();
    }
}
