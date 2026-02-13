using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record RefreshTokenCreatedEvent(
    Guid RefreshTokenId,
    Guid UserId,
    DateTime ExpiresAt,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
