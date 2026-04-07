using Organization.Domain.Common;

namespace Organization.Domain.Events;

public sealed record OrganizationUpdatedEvent(Guid OrganizationId, DateTime OccurredAt) : DomainEvent(OccurredAt);
