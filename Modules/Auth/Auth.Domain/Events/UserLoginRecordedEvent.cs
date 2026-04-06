using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserLoginRecordedEvent(
    Guid UserId,
    string? Username,
    string? IpAddress,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

