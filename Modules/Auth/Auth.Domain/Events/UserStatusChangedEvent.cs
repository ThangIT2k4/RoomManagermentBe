using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserStatusChangedEvent(
    Guid UserId,
    short FromStatus,
    short ToStatus,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

