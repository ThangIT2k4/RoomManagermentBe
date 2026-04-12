using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.GetCommissionPolicies;

public sealed class GetCommissionPoliciesQueryHandler(ICrmApplicationService crm)
    : IAppRequestHandler<GetCommissionPoliciesQuery, Result<GetCommissionPoliciesResult>>
{
    public Task<Result<GetCommissionPoliciesResult>> Handle(GetCommissionPoliciesQuery request, CancellationToken cancellationToken)
        => crm.GetCommissionPoliciesAsync(request, cancellationToken);
}
