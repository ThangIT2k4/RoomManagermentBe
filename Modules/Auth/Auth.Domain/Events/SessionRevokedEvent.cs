using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record SessionRevokedEvent(
    string SessionId,
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

