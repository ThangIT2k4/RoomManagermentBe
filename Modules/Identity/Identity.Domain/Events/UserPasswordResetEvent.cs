using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record UserPasswordResetEvent(
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

