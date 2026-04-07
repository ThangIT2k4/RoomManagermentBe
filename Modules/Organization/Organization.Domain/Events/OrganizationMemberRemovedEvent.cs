using Organization.Domain.Common;

namespace Organization.Domain.Events;

public sealed record OrganizationMemberRemovedEvent(Guid OrganizationId, Guid UserId, DateTime OccurredAt) : DomainEvent(OccurredAt);
