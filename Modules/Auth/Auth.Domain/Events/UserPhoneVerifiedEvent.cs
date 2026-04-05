using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserPhoneVerifiedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

