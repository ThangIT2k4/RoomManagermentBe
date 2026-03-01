using Identity.API.Common;
using Identity.API.Requests;
using Identity.Application.Features.Auth.CreateRefreshToken;
using Identity.Application.Features.Auth.RevokeRefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateRefreshToken(
        [FromBody] CreateRefreshTokenCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created(string.Empty, null);
        }

        return result.ToActionResult();
    }

    [HttpPost("refresh-token/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RevokeRefreshToken(
        [FromBody] RevokeRefreshTokenRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.Token);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
