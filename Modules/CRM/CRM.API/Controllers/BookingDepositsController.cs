using CRM.API.Common;
using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/booking-deposits")]
public sealed class BookingDepositsController(ICrmApplicationService crmService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookingDepositDto>> Create([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        => (await crmService.CreateBookingAsync(command, cancellationToken)).ToActionResult();

    [HttpPost("{bookingId:guid}/approve")]
    public async Task<ActionResult<BookingDepositDto>> Approve([FromRoute] Guid bookingId, [FromBody] ApproveBookingCommand command, CancellationToken cancellationToken)
        => (await crmService.ApproveBookingAsync(command with { BookingId = bookingId }, cancellationToken)).ToActionResult();

    [HttpPost("{bookingId:guid}/confirm-payment")]
    public async Task<ActionResult<BookingDepositDto>> Pay([FromRoute] Guid bookingId, [FromBody] PayBookingCommand command, CancellationToken cancellationToken)
        => (await crmService.PayBookingAsync(command with { BookingId = bookingId }, cancellationToken)).ToActionResult();

    [HttpGet]
    public async Task<ActionResult<GetBookingsResult>> List([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await crmService.GetBookingsAsync(new GetBookingsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken)).ToActionResult();
}
