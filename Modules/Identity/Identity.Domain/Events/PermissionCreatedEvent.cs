using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record PermissionCreatedEvent(
    Guid PermissionId,
    string Code,
    string Name,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
