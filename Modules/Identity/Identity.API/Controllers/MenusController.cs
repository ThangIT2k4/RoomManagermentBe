using Identity.API.Common;
using Identity.Application.Common;
using Identity.Application.Features.Menus.CreateMenu;
using Identity.Application.Features.Menus.GetMenuById;
using Identity.Application.Features.Menus.GetMenusByParent;
using Identity.Application.Features.Menus.GetMenusPaged;
using Identity.Application.Features.Menus.SetMenuActive;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.API.Requests;

namespace Identity.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MenusController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateMenuResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateMenuResult>> CreateMenu(
        [FromBody] CreateMenuCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetMenuById), new { id = result.Value?.Id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<MenuPagedListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<MenuPagedListItemDto>>> GetMenusPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var queryFilter = FilterParser.Parse(filter);
        var query = new GetMenusPagedQuery(page, pageSize, queryFilter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("by-parent")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<MenuListItemDto>>> GetMenusByParent(
        [FromQuery] Guid? parentId = null,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var queryFilter = FilterParser.Parse(filter);
        var query = new GetMenusByParentQuery(parentId, queryFilter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{menuId}/active")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetMenuActive(
        [FromRoute] Guid menuId,
        [FromBody] SetMenuActiveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetMenuActiveCommand(menuId, request.IsActive);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuDto>> GetMenuById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetMenuByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
