using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.GetViewings;

public sealed class GetViewingsQueryHandler(ICrmApplicationService crm)
    : IAppRequestHandler<GetViewingsQuery, Result<GetViewingsResult>>
{
    public Task<Result<GetViewingsResult>> Handle(GetViewingsQuery request, CancellationToken cancellationToken)
        => crm.GetViewingsAsync(request, cancellationToken);
}
