using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record RoleCreatedEvent(
    Guid RoleId,
    string Code,
    string Name,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
