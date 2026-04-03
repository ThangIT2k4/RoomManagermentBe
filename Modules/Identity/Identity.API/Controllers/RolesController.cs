using Identity.API.Common;
using Identity.API.Requests;
using Identity.Application.Common;
using Identity.Application.Features.Roles.AssignPermissionsToRole;
using Identity.Application.Features.Roles.CreateRole;
using Identity.Application.Features.Roles.GetRoleById;
using Identity.Application.Features.Roles.GetRolesPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateRoleResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateRoleResult>> CreateRole(
        [FromBody] CreateRoleCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetRoleById), new { id = result.Value?.Id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RoleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<RoleListItemDto>>> GetRolesPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var queryFilter = FilterParser.Parse(filter);
        var query = new GetRolesPagedQuery(page, pageSize, queryFilter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{roleId}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AssignPermissionsToRole(
        [FromRoute] Guid roleId,
        [FromBody] AssignPermissionsToRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignPermissionsToRoleCommand(roleId, request.PermissionIds);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetRoleById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
