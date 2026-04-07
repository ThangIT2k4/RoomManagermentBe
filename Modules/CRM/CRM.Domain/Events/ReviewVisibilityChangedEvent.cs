using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record ReviewVisibilityChangedEvent(
    Guid ReviewId,
    Guid OrganizationId,
    bool IsPublic,
    string? Reason,
    DateTimeOffset OccurredOn) : DomainEvent;
