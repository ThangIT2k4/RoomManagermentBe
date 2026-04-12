using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.ApproveBooking;

public sealed class ApproveBookingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<ApproveBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
        => crm.ApproveBookingAsync(request, cancellationToken);
}
