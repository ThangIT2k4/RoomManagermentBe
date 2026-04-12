using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.ApproveCommission;

public sealed record ApproveCommissionCommand(Guid CommissionEventId, Guid ApprovedBy, DateTime? ApprovedAt = null)
    : IAppRequest<Result<CommissionEventDto>>;
