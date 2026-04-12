using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid LeadId,
    decimal DepositAmount,
    string Currency,
    DateTime ExpiredAt,
    Guid RequestedBy)
    : IAppRequest<Result<BookingDepositDto>>;
