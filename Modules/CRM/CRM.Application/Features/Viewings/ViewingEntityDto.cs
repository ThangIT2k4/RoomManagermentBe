namespace CRM.Application.Features.Viewings;

public sealed record ViewingEntityDto(
    Guid Id,
    Guid OrganizationId,
    Guid? LeadId,
    Guid? AgentId,
    DateTime ScheduleAt,
    string Status,
    string? Note,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
