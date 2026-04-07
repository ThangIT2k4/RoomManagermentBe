using Organization.Domain.Common;

namespace Organization.Domain.Events;

public sealed record OrganizationInvitationSentEvent(
    Guid OrganizationId,
    string Email,
    string Token,
    DateTime ExpiresAt,
    DateTime OccurredAt) : DomainEvent(OccurredAt);
