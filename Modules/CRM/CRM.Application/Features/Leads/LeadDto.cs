namespace CRM.Application.Features.Leads;

public sealed record LeadDto(
    Guid Id,
    Guid OrganizationId,
    string? FullName,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
