using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.PayBooking;

public sealed class PayBookingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<PayBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(PayBookingCommand request, CancellationToken cancellationToken)
        => crm.PayBookingAsync(request, cancellationToken);
}
