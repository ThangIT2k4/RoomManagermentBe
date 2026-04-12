using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.GetCommissionEvents;

public sealed class GetCommissionEventsQueryHandler(ICrmApplicationService crm)
    : IAppRequestHandler<GetCommissionEventsQuery, Result<GetCommissionEventsResult>>
{
    public Task<Result<GetCommissionEventsResult>> Handle(GetCommissionEventsQuery request, CancellationToken cancellationToken)
        => crm.GetCommissionEventsAsync(request, cancellationToken);
}
