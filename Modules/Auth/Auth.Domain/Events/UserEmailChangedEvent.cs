using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserEmailChangedEvent(
    Guid UserId,
    string Email,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

