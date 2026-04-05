using System.Security.Claims;
using Auth.API.Common;
using Auth.Application.Dtos;
using Auth.Application.Features.Auth.Sessions.LogoutAllSessions;
using Auth.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public sealed class SessionsController(IAuthApplicationService authService, IMediator mediator) : ControllerBase
{
    [HttpGet("active")]
    [ProducesResponseType(typeof(PagedSessionsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedSessionsResult>> GetActiveSessions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var result = await authService.GetActiveSessionsAsync(new GetActiveSessionsRequest(userId.Value, pageNumber, pageSize), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var result = await mediator.Send(new LogoutAllSessionsCommand(userId.Value), cancellationToken);
        return result.ToActionResult();
    }

    private Guid? ResolveUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
