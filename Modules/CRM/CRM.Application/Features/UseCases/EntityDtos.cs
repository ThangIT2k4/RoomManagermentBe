namespace CRM.Application.Features.UseCases;

public sealed record ViewingEntityDto(Guid Id, Guid OrganizationId, Guid? LeadId, Guid? AgentId, DateTime ScheduleAt, string Status, string? Note, DateTime CreatedAt, DateTime? UpdatedAt);
public sealed record BookingDepositDto(Guid Id, Guid OrganizationId, Guid? LeadId, Guid? ViewingId, decimal Amount, string? DepositType, string PaymentStatus, DateTime CreatedAt, DateTime? UpdatedAt);
public sealed record ReviewDto(Guid Id, Guid OrganizationId, Guid UnitId, Guid UserId, short Rating, string? Content, bool IsPublic, DateTime CreatedAt, DateTime? UpdatedAt);
public sealed record ReviewReplyDto(Guid Id, Guid ReviewId, Guid UserId, string Content, DateTime CreatedAt, DateTime? UpdatedAt);
public sealed record CommissionPolicyDto(Guid Id, Guid OrganizationId, string Title, string CalcType, string TriggerEvent, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);
public sealed record CommissionEventDto(Guid Id, Guid OrganizationId, Guid? PolicyId, Guid? AgentId, decimal CommissionTotal, DateTime OccurredAt, string Status, string? TriggerEvent, DateTime CreatedAt, DateTime? UpdatedAt);
