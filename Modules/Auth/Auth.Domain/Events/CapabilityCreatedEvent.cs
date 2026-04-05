using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record CapabilityCreatedEvent(
    Guid CapabilityId,
    string KeyCode,
    string Name,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

