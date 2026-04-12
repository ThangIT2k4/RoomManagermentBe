using CRM.Application.Features.UseCases;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/booking-deposits")]
public sealed class BookingDepositsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookingDepositDto>> Create([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await mediator.Send(command, cancellationToken));

    [HttpPost("{bookingId:guid}/approve")]
    public async Task<ActionResult<BookingDepositDto>> Approve([FromRoute] Guid bookingId, [FromBody] ApproveBookingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await mediator.Send(command with { BookingId = bookingId }, cancellationToken));

    [HttpPost("{bookingId:guid}/confirm-payment")]
    public async Task<ActionResult<BookingDepositDto>> Pay([FromRoute] Guid bookingId, [FromBody] PayBookingCommand command, CancellationToken cancellationToken)
        => this.ToActionResult(await mediator.Send(command with { BookingId = bookingId }, cancellationToken));

    [HttpGet]
    public async Task<ActionResult<GetBookingsResult>> List([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => this.ToActionResult(await mediator.Send(new GetBookingsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken));
}
