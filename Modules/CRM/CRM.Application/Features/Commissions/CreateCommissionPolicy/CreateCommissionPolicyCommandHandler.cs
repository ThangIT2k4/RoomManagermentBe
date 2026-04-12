using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.CreateCommissionPolicy;

public sealed class CreateCommissionPolicyCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CreateCommissionPolicyCommand, Result<CommissionPolicyDto>>
{
    public Task<Result<CommissionPolicyDto>> Handle(CreateCommissionPolicyCommand request, CancellationToken cancellationToken)
        => crm.CreateCommissionPolicyAsync(request, cancellationToken);
}
