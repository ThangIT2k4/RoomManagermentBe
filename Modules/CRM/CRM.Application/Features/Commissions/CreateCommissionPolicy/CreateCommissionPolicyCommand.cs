using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.CreateCommissionPolicy;

public sealed record CreateCommissionPolicyCommand(
    Guid OrganizationId,
    string Name,
    string PolicyType,
    decimal Rate,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo = null,
    Guid? CreatedBy = null)
    : IAppRequest<Result<CommissionPolicyDto>>;
