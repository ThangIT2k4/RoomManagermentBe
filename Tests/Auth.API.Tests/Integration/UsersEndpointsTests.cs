using System.Net;
using System.Net.Http.Headers;
using Auth.Application.Dtos;
using Moq;
using RoomManagerment.Shared.Common;

namespace Auth.API.Tests.Integration;

public sealed class UsersEndpointsTests
{
    [Fact]
    public async Task GetUsers_ShouldReturnUnprocessableEntity_WhenSearchTermIsUnsafe()
    {
        await using var host = new AuthApiTestHost();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/users?searchTerm=union select * from users");
        request.Headers.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());

        var response = await host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ShouldClampPaging_WhenParametersOutOfRange()
    {
        await using var host = new AuthApiTestHost();
        GetUsersRequest? captured = null;

        host.AuthServiceMock
            .Setup(x => x.GetUsersAsync(It.IsAny<GetUsersRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetUsersRequest, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(Result<PagedUsersResult>.Success(new PagedUsersResult([], 0, 1, 500, 0)));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/users?pageNumber=0&pageSize=999");
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);
        request.Headers.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());

        var response = await host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(1, captured!.PageNumber);
        Assert.Equal(500, captured.PageSize);
        host.AuthServiceMock.Verify(x => x.GetUsersAsync(It.IsAny<GetUsersRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUnauthorized_WhenNoAuthHeader()
    {
        await using var host = new AuthApiTestHost();

        var response = await host.Client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
