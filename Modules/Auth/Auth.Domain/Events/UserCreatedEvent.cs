using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Email,
    string? Username,
    string? Phone,
    short Status,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

