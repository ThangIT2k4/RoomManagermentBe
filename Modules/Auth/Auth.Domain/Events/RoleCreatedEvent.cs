using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record RoleCreatedEvent(
    Guid RoleId,
    string KeyCode,
    string Name,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

