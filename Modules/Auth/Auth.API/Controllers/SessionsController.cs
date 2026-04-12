using System.Security.Claims;
using Auth.Application.Features.Auth.Sessions.GetActiveSessions;
using Auth.Application.Features.Auth.Sessions.LogoutAllSessions;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public sealed class SessionsController(IAppSender sender) : ControllerBase
{
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<PagedSessionsResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedSessionsResult>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedSessionsResult>>> GetActiveSessions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<PagedSessionsResult>("User identity is missing.");
        }

        var result = await sender.Send(new GetActiveSessionsQuery(userId.Value, pageNumber, pageSize), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPost("logout-all")]
    [ProducesResponseType(typeof(ApiResponse<AuthLogoutAllSessionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthLogoutAllSessionsResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthLogoutAllSessionsResponse>>> LogoutAll(CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<AuthLogoutAllSessionsResponse>("User identity is missing.");
        }

        var result = await sender.Send(new LogoutAllSessionsCommand(userId.Value), cancellationToken);
        return this.ToApiVoidActionResult<AuthLogoutAllSessionsResponse>(result);
    }

    private Guid? ResolveUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
