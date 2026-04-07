using Organization.Domain.Common;

namespace Organization.Domain.Events;

public sealed record OrganizationMemberJoinedEvent(
    Guid OrganizationId,
    Guid UserId,
    Guid? RoleId,
    DateTime OccurredAt) : DomainEvent(OccurredAt);
