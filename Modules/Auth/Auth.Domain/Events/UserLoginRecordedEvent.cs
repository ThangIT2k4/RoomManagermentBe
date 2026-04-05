using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserLoginRecordedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

