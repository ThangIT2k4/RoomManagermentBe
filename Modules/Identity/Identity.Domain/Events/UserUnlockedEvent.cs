using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserUnlockedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
