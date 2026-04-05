using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record UserProfileUpdatedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

