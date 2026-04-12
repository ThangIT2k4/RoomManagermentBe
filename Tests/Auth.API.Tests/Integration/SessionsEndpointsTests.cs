using System.Net;
using Auth.Application.Dtos;
using Moq;
using RoomManagerment.Shared.Common;

namespace Auth.API.Tests.Integration;

public sealed class SessionsEndpointsTests
{
    [Fact]
    public async Task GetActiveSessions_ShouldReturnUnauthorized_WhenNoIdentity()
    {
        await using var host = new AuthApiTestHost();

        var response = await host.Client.GetAsync("/api/sessions/active");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveSessions_ShouldReturnOk_WhenAuthenticated()
    {
        await using var host = new AuthApiTestHost();
        var userId = Guid.NewGuid();
        GetActiveSessionsRequest? captured = null;

        host.AuthServiceMock
            .Setup(x => x.GetActiveSessionsAsync(It.IsAny<GetActiveSessionsRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetActiveSessionsRequest, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(Result<PagedSessionsResult>.Success(new PagedSessionsResult([], 0, 1, 20, 0)));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/sessions/active");
        request.Headers.Add(TestAuthHandler.UserIdHeader, userId.ToString());

        var response = await host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(userId, captured!.UserId);
        Assert.Equal(1, captured.PageNumber);
        Assert.Equal(20, captured.PageSize);
        host.AuthServiceMock.Verify(x => x.GetActiveSessionsAsync(It.IsAny<GetActiveSessionsRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
