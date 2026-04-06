using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserEmailVerifiedEvent(
    Guid UserId,
    string Email,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
