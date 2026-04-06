using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record LeadCreatedEvent(
    Guid LeadId,
    Guid OrganizationId,
    string? FullName,
    string Status,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
