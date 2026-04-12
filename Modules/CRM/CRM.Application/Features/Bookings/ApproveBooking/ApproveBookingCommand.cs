using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.ApproveBooking;

public sealed record ApproveBookingCommand(Guid BookingId, Guid ApprovedBy, DateTime? ApprovedAt = null)
    : IAppRequest<Result<BookingDepositDto>>;
