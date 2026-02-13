using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserCreatedEvent(
    Guid UserId,
    string Username,
    string Email,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
