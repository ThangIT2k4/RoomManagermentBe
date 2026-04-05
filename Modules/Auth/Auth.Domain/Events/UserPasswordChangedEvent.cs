using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserPasswordChangedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

