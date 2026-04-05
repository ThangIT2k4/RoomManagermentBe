using Auth.API.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/capabilities")]
public sealed class CapabilitiesController(IAuthApplicationService authService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedCapabilitiesResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedCapabilitiesResult>> GetCapabilities(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await authService.GetCapabilitiesAsync(new GetCapabilitiesRequest(searchTerm, pageNumber, pageSize), cancellationToken);
        return result.ToActionResult();
    }
}
