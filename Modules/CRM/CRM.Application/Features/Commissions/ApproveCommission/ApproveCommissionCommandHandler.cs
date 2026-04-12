using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.ApproveCommission;

public sealed class ApproveCommissionCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<ApproveCommissionCommand, Result<CommissionEventDto>>
{
    public Task<Result<CommissionEventDto>> Handle(ApproveCommissionCommand request, CancellationToken cancellationToken)
        => crm.ApproveCommissionAsync(request, cancellationToken);
}
