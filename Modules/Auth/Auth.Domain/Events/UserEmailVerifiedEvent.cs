using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserEmailVerifiedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

