using Auth.API.Controllers;
using Auth.API.Requests;
using Auth.API.Validators;
using Auth.Application.Dtos;
using Auth.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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
        MediatorMock = new Mock<IMediator>(MockBehavior.Strict);

        builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services
            .AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IAuthApplicationService>(_ => AuthServiceMock.Object);
        builder.Services.AddScoped<IMediator>(_ => MediatorMock.Object);

        builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<LoginApiRequest>, LoginApiRequestValidator>();
        builder.Services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
        builder.Services.AddScoped<IValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>();
        builder.Services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();
        builder.Services.AddScoped<IValidator<SendOtpRequest>, SendOtpRequestValidator>();
        builder.Services.AddScoped<IValidator<VerifyOtpRequest>, VerifyOtpRequestValidator>();
        builder.Services.AddScoped<IValidator<ResendOtpRequest>, ResendOtpRequestValidator>();
        builder.Services.AddScoped<IValidator<VerifyEmailRequest>, VerifyEmailRequestValidator>();
        builder.Services.AddScoped<IValidator<UpdateUserApiRequest>, UpdateUserApiRequestValidator>();
        builder.Services.AddScoped<IValidator<AssignRoleApiRequest>, AssignRoleApiRequestValidator>();
        builder.Services.AddScoped<IValidator<UpdateProfileApiRequest>, UpdateProfileApiRequestValidator>();
        builder.Services.AddScoped<IValidator<UploadAvatarApiRequest>, UploadAvatarApiRequestValidator>();

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
    public Mock<IMediator> MediatorMock { get; }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return _app.DisposeAsync();
    }
}
