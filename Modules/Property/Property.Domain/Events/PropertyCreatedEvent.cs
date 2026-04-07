using Property.Domain.Common;

namespace Property.Domain.Events;

public sealed record PropertyCreatedEvent(Guid PropertyId, Guid OrganizationId, string Name, DateTimeOffset OccurredOn)
    : DomainEvent(OccurredOn);
