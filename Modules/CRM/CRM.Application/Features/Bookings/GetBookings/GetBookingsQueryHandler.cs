using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.GetBookings;

public sealed class GetBookingsQueryHandler(ICrmApplicationService crm)
    : IAppRequestHandler<GetBookingsQuery, Result<GetBookingsResult>>
{
    public Task<Result<GetBookingsResult>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        => crm.GetBookingsAsync(request, cancellationToken);
}
