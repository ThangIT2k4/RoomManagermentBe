using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserPasswordChangedEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

