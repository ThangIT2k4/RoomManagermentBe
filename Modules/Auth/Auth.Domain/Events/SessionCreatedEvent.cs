using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record SessionCreatedEvent(
    string SessionId,
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

