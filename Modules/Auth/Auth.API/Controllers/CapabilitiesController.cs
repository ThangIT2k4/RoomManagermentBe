using Auth.Application.Features.Auth.Capabilities.GetCapabilities;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/capabilities")]
public sealed class CapabilitiesController(IAppSender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedCapabilitiesResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedCapabilitiesResult>> GetCapabilities(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetCapabilitiesQuery(searchTerm, pageNumber, pageSize), cancellationToken);
        return this.ToActionResult(result);
    }
}
