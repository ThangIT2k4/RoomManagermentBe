using Identity.API.Common;
using Identity.Application.Common;
using Identity.Application.Features.Permissions.CreatePermission;
using Identity.Application.Features.Permissions.GetPermissionById;
using Identity.Application.Features.Permissions.GetPermissionsPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PermissionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatePermissionResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatePermissionResult>> CreatePermission(
        [FromBody] CreatePermissionCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetPermissionById), new { id = result.Value?.Id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PermissionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<PermissionListItemDto>>> GetPermissionsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var queryFilter = FilterParser.Parse(filter);
        var query = new GetPermissionsPagedQuery(page, pageSize, queryFilter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetPermissionById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPermissionByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
