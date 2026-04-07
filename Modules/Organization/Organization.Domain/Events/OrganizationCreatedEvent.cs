using Organization.Domain.Common;

namespace Organization.Domain.Events;

public sealed record OrganizationCreatedEvent(Guid OrganizationId, DateTime OccurredAt) : DomainEvent(OccurredAt);
