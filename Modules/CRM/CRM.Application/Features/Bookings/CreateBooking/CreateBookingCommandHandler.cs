using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CreateBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        => crm.CreateBookingAsync(request, cancellationToken);
}
