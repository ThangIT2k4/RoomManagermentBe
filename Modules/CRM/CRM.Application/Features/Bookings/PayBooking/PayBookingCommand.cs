using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.PayBooking;

public sealed record PayBookingCommand(
    Guid BookingId,
    decimal PaidAmount,
    DateTime PaidAt,
    string? PaymentMethod,
    Guid RequestedBy)
    : IAppRequest<Result<BookingDepositDto>>;
