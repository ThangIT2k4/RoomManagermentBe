using CRM.Application.Features.Bookings;
using CRM.Application.Features.Bookings.ApproveBooking;
using CRM.Application.Features.Bookings.CreateBooking;
using CRM.Application.Features.Bookings.GetBookings;
using CRM.Application.Features.Bookings.PayBooking;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/booking-deposits")]
public sealed class BookingDepositsController(IAppSender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDepositDto>>> Create([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command, cancellationToken));

    [HttpPost("{bookingId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<BookingDepositDto>>> Approve([FromRoute] Guid bookingId, [FromBody] ApproveBookingCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command with { BookingId = bookingId }, cancellationToken));

    [HttpPost("{bookingId:guid}/confirm-payment")]
    public async Task<ActionResult<ApiResponse<BookingDepositDto>>> Pay([FromRoute] Guid bookingId, [FromBody] PayBookingCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command with { BookingId = bookingId }, cancellationToken));

    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetBookingsResult>>> List(
        [FromQuery] Guid organizationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToApiActionResult(await sender.Send(
            new GetBookingsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)),
            cancellationToken));
}
