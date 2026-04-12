using System.Security.Claims;
using Auth.API.Requests;
using Auth.Application.Features.Auth.ChangeUserStatus;
using Auth.Application.Features.Auth.Roles.GetUserRoles;
using Auth.Application.Features.Auth.Users.GetUserById;
using Auth.Application.Features.Auth.Users.GetUsers;
using Auth.Application.Features.Users.AssignRole;
using Auth.Application.Features.Users.DeleteUser;
using Auth.Application.Features.Users.RemoveRole;
using Auth.Application.Features.Users.UpdateUser;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(IAppSender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedUsersResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedUsersResult>> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetUsersQuery(searchTerm, pageNumber, pageSize, includeDeleted),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateUserCommand(userId, request.Email, request.Username, request.Phone, request.Status),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var actorId = ResolveActorUserId();
        if (actorId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var result = await sender.Send(new DeleteUserCommand(userId, actorId.Value), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPatch("{userId:guid}/status")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ChangeStatus([FromRoute] Guid userId, [FromBody] ChangeUserStatusApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeUserStatusCommand(userId, request.Status), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{userId:guid}/ban")]
    public Task<ActionResult<UserDto>> Ban([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(2), cancellationToken);

    [HttpPost("{userId:guid}/unban")]
    public Task<ActionResult<UserDto>> Unban([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(1), cancellationToken);

    [HttpPost("{userId:guid}/activate")]
    public Task<ActionResult<UserDto>> Activate([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(1), cancellationToken);

    [HttpPost("{userId:guid}/deactivate")]
    public Task<ActionResult<UserDto>> Deactivate([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(0), cancellationToken);

    [HttpPost("{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> AssignRole([FromRoute] Guid userId, [FromBody] AssignRoleApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignRoleCommand(request.OrganizationId, userId, request.RoleId),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveRole([FromRoute] Guid userId, [FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveRoleCommand(organizationId, userId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles([FromRoute] Guid userId, [FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserRolesQuery(userId, organizationId), cancellationToken);
        return this.ToActionResult(result);
    }

    private Guid? ResolveActorUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}
