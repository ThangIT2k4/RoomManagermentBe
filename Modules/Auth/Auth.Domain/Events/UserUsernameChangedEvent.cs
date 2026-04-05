using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserUsernameChangedEvent(
    Guid UserId,
    string? Username,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

