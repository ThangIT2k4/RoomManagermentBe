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
using RoomManagerment.Shared.Http;
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
    [ProducesResponseType(typeof(ApiResponse<PagedUsersResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedUsersResult>>> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetUsersQuery(searchTerm, pageNumber, pageSize, includeDeleted),
            cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateUserCommand(userId, request.Email, request.Username, request.Phone, request.Status),
            cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthDeleteUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthDeleteUserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthDeleteUserResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthDeleteUserResponse>>> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var actorId = ResolveActorUserId();
        if (actorId is null)
        {
            return this.ApiUnauthorized<AuthDeleteUserResponse>("User identity is missing.");
        }

        var result = await sender.Send(new DeleteUserCommand(userId, actorId.Value), cancellationToken);
        return this.ToApiVoidActionResult<AuthDeleteUserResponse>(result);
    }

    [HttpPatch("{userId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> ChangeStatus([FromRoute] Guid userId, [FromBody] ChangeUserStatusApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeUserStatusCommand(userId, request.Status), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPost("{userId:guid}/ban")]
    public Task<ActionResult<ApiResponse<UserDto>>> Ban([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(2), cancellationToken);

    [HttpPost("{userId:guid}/unban")]
    public Task<ActionResult<ApiResponse<UserDto>>> Unban([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(1), cancellationToken);

    [HttpPost("{userId:guid}/activate")]
    public Task<ActionResult<ApiResponse<UserDto>>> Activate([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(1), cancellationToken);

    [HttpPost("{userId:guid}/deactivate")]
    public Task<ActionResult<ApiResponse<UserDto>>> Deactivate([FromRoute] Guid userId, CancellationToken cancellationToken)
        => ChangeStatus(userId, new ChangeUserStatusApiRequest(0), cancellationToken);

    [HttpPost("{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<AuthAssignRoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthAssignRoleResponse>>> AssignRole([FromRoute] Guid userId, [FromBody] AssignRoleApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignRoleCommand(request.OrganizationId, userId, request.RoleId),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthAssignRoleResponse>(result);
    }

    [HttpDelete("{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<AuthRemoveRoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthRemoveRoleResponse>>> RemoveRole([FromRoute] Guid userId, [FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveRoleCommand(organizationId, userId), cancellationToken);
        return this.ToApiVoidActionResult<AuthRemoveRoleResponse>(result);
    }

    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleDto>>>> GetRoles([FromRoute] Guid userId, [FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserRolesQuery(userId, organizationId), cancellationToken);
        return this.ToApiActionResult(result);
    }

    private Guid? ResolveActorUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}
