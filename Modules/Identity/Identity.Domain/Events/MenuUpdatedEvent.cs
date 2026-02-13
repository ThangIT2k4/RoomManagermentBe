using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record MenuUpdatedEvent(
    Guid MenuId,
    string Label,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
