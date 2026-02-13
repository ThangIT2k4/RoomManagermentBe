using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserActivatedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
