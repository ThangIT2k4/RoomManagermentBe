using Identity.API.Common;
using Identity.API.Requests;
using Identity.Application.Common;
using Identity.Application.Features.Users.ActivateUser;
using Identity.Application.Features.Users.AssignRolesToUser;
using Identity.Application.Features.Users.GetUserById;
using Identity.Application.Features.Users.GetUsersPaged;
using Identity.Application.Features.Users.RegisterUser;
using Identity.Application.Features.Users.SetUserPermission;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterUserResult>> RegisterUser(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetUserById), new { id = result.Value?.Id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<UserListItemDto>>> GetUsersPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var queryFilter = FilterParser.Parse(filter);
        var query = new GetUsersPagedQuery(page, pageSize, queryFilter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{userId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ActivateUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AssignRolesToUser(
        [FromRoute] Guid userId,
        [FromBody] AssignRolesToUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignRolesToUserCommand(userId, request.RoleIds);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

