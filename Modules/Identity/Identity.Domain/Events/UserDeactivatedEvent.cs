using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserDeactivatedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
