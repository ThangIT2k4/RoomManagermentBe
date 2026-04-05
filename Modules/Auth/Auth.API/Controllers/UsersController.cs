using System.Security.Claims;
using Auth.API.Common;
using Auth.API.Requests;
using Auth.API.Validators;
using Auth.Application.Dtos;
using Auth.Application.Services;
using Auth.Domain.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(
    IAuthApplicationService authService,
    IValidator<UpdateUserApiRequest> updateUserValidator,
    IValidator<AssignRoleApiRequest> assignRoleValidator) : ControllerBase
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
        if (!ValidationGuards.BeSafeText(searchTerm))
        {
            return BadRequest(new { message = "Search term contains unsafe content." });
        }

        var paging = PagingInput.Create(pageNumber, pageSize);
        var safeSearchTerm = SearchInput.Normalize(searchTerm);

        var result = await authService.GetUsersAsync(new GetUsersRequest(safeSearchTerm, paging.PageNumber, paging.PageSize, includeDeleted), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await authService.GetUserByIdAsync(new GetUserByIdRequest(userId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserApiRequest request, CancellationToken cancellationToken)
    {
        var validation = await updateUserValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var command = new UpdateUserRequest(userId, request.Email, request.Username, request.Phone, request.Status);
        var result = await authService.UpdateUserAsync(command, cancellationToken);
        return result.ToActionResult();
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

        var result = await authService.DeleteUserAsync(new DeleteUserRequest(userId, actorId.Value), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{userId:guid}/status")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ChangeStatus([FromRoute] Guid userId, [FromBody] ChangeUserStatusApiRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ChangeUserStatusAsync(new ChangeUserStatusRequest(userId, request.Status), cancellationToken);
        return result.ToActionResult();
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
        var validation = await assignRoleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await authService.AssignRoleAsync(new AssignRoleRequest(request.OrganizationId, userId, request.RoleId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveRole([FromRoute] Guid userId, [FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        if (organizationId == Guid.Empty)
        {
            return BadRequest(new { message = "organizationId is required." });
        }

        var result = await authService.RemoveRoleAsync(new RemoveRoleRequest(organizationId, userId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles([FromRoute] Guid userId, [FromQuery] Guid? organizationId, CancellationToken cancellationToken)
    {
        var result = await authService.GetUserRolesAsync(new GetUserRolesRequest(userId, organizationId), cancellationToken);
        return result.ToActionResult();
    }

    private Guid? ResolveActorUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    private static object ToValidationErrorPayload(ValidationResult validationResult)
    {
        return new
        {
            message = "Validation failed",
            errors = validationResult.Errors.Select(error => new
            {
                field = error.PropertyName,
                error = error.ErrorMessage
            })
        };
    }
}
