using System.Net;
using System.Net.Http.Json;
using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Features.Auth.Users.GetUserById;
using Moq;

namespace Auth.API.Tests.Integration;

public sealed class AuthEndpointsTests
{
    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPayloadInvalid()
    {
        await using var host = new AuthApiTestHost();

        var response = await host.Client.PostAsJsonAsync("/api/auth/login", new
        {
            login = "admin' OR 1=1 --",
            password = "Password1",
            rememberMe = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        host.MediatorMock.Verify(
            x => x.Send(It.IsAny<Auth.Application.Features.Auth.Login.LoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendOtp_ShouldReturnBadRequest_WhenEmailDoesNotMatchAuthenticatedUser()
    {
        await using var host = new AuthApiTestHost();
        var userId = Guid.NewGuid();
        var existingUser = new UserDto(
            userId,
            "owner@example.com",
            "owner",
            1,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null);

        host.MediatorMock
            .Setup(x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(existingUser));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/send-otp")
        {
            Content = JsonContent.Create(new
            {
                email = "other@example.com",
                purpose = 1
            })
        };
        request.Headers.Add(TestAuthHandler.UserIdHeader, userId.ToString());

        var response = await host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        host.MediatorMock.Verify(x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        host.MediatorMock.Verify(
            x => x.Send(It.IsAny<Auth.Application.Features.Auth.SendOtp.SendOtpCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
