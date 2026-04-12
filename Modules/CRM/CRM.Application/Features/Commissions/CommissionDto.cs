namespace CRM.Application.Features.Commissions;

public sealed record CommissionPolicyDto(
    Guid Id,
    Guid OrganizationId,
    string Title,
    string CalcType,
    string TriggerEvent,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CommissionEventDto(
    Guid Id,
    Guid OrganizationId,
    Guid? PolicyId,
    Guid? AgentId,
    decimal CommissionTotal,
    DateTime OccurredAt,
    string Status,
    string? TriggerEvent,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
