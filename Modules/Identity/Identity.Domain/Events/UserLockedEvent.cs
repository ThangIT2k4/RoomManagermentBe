using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserLockedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
