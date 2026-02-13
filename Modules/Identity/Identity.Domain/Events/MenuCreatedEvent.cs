using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record MenuCreatedEvent(
    Guid MenuId,
    string Code,
    string Label,
    int OrderIndex,
    Guid? ParentId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
