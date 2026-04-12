using System.Net;
using System.Net.Http.Json;
using Auth.Application.Dtos;
using Moq;
using RoomManagerment.Shared.Common;

namespace Auth.API.Tests.Integration;

public sealed class AuthEndpointsTests
{
    [Fact]
    public async Task Login_ShouldReturnUnprocessableEntity_WhenPayloadInvalid()
    {
        await using var host = new AuthApiTestHost();

        var response = await host.Client.PostAsJsonAsync("/api/auth/login", new
        {
            login = "admin' OR 1=1 --",
            password = "Password1",
            rememberMe = false
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        host.AuthServiceMock.Verify(
            x => x.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()),
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

        host.AuthServiceMock
            .Setup(x => x.GetUserByIdAsync(It.IsAny<GetUserByIdRequest>(), It.IsAny<CancellationToken>()))
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
        host.AuthServiceMock.Verify(x => x.GetUserByIdAsync(It.IsAny<GetUserByIdRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        host.AuthServiceMock.Verify(
            x => x.SendOtpAsync(It.IsAny<SendOtpRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
