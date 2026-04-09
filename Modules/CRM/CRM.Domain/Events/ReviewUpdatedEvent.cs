using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record ReviewUpdatedEvent(
    Guid ReviewId,
    Guid OrganizationId,
    short Rating,
    bool IsPublic,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
