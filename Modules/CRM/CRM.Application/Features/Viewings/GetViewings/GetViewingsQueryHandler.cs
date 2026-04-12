using CRM.Application.Features.Viewings.GetViewings;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Viewings.GetViewings;

public sealed class GetViewingsQueryHandler(ICrmApplicationService crm)
    : IRequestHandler<GetViewingsQuery, Result<GetViewingsResult>>
{
    public Task<Result<GetViewingsResult>> Handle(GetViewingsQuery request, CancellationToken cancellationToken)
        => crm.GetViewingsAsync(request, cancellationToken);
}
