using CRM.API.Common;
using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/viewings")]
public sealed class ViewingsController(ICrmApplicationService crmService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ViewingEntityDto>> Create([FromBody] CreateViewingCommand command, CancellationToken cancellationToken)
        => (await crmService.CreateViewingAsync(command, cancellationToken)).ToActionResult();

    [HttpPost("{viewingId:guid}/confirm")]
    public async Task<ActionResult<ViewingEntityDto>> Confirm([FromRoute] Guid viewingId, CancellationToken cancellationToken)
        => (await crmService.ConfirmViewingAsync(viewingId, cancellationToken)).ToActionResult();

    [HttpPost("{viewingId:guid}/complete")]
    public async Task<ActionResult<ViewingEntityDto>> Complete([FromRoute] Guid viewingId, [FromBody] CompleteViewingCommand command, CancellationToken cancellationToken)
        => (await crmService.CompleteViewingAsync(command with { ViewingId = viewingId }, cancellationToken)).ToActionResult();

    [HttpPost("{viewingId:guid}/cancel")]
    public async Task<ActionResult<ViewingEntityDto>> Cancel([FromRoute] Guid viewingId, [FromBody] CancelViewingCommand command, CancellationToken cancellationToken)
        => (await crmService.CancelViewingAsync(command with { ViewingId = viewingId }, cancellationToken)).ToActionResult();

    [HttpGet]
    public async Task<ActionResult<GetViewingsResult>> List([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await crmService.GetViewingsAsync(new GetViewingsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken)).ToActionResult();
}
