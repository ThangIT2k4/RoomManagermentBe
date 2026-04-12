using CRM.Application.Features.Shared;
using CRM.Application.Features.Viewings;
using CRM.Application.Features.Viewings.CancelViewing;
using CRM.Application.Features.Viewings.CompleteViewing;
using CRM.Application.Features.Viewings.ConfirmViewing;
using CRM.Application.Features.Viewings.CreateViewing;
using CRM.Application.Features.Viewings.GetViewings;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/viewings")]
public sealed class ViewingsController(IAppSender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ViewingEntityDto>> Create([FromBody] CreateViewingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await sender.Send(command, cancellationToken));

    [HttpPost("{viewingId:guid}/confirm")]
    public async Task<ActionResult<ViewingEntityDto>> Confirm([FromRoute] Guid viewingId, CancellationToken cancellationToken)
        => this.ToActionResult(await sender.Send(new ConfirmViewingCommand(viewingId), cancellationToken));

    [HttpPost("{viewingId:guid}/complete")]
    public async Task<ActionResult<ViewingEntityDto>> Complete([FromRoute] Guid viewingId, [FromBody] CompleteViewingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await sender.Send(command with { ViewingId = viewingId }, cancellationToken));

    [HttpPost("{viewingId:guid}/cancel")]
    public async Task<ActionResult<ViewingEntityDto>> Cancel([FromRoute] Guid viewingId, [FromBody] CancelViewingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await sender.Send(command with { ViewingId = viewingId }, cancellationToken));

    [HttpGet]
    public async Task<ActionResult<GetViewingsResult>> List(
        [FromQuery] Guid organizationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToActionResult(await sender.Send(
            new GetViewingsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)),
            cancellationToken));
}
