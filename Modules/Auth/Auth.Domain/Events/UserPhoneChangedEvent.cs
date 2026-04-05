using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserPhoneChangedEvent(
    Guid UserId,
    string? Phone,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

