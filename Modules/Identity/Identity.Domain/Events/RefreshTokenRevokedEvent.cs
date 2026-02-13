using Identity.Domain.Common;

namespace Identity.Domain.Events;

public record RefreshTokenRevokedEvent(
    Guid RefreshTokenId,
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
