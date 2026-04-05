using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record EmailOtpIssuedEvent(
    Guid EmailOtpId,
    string Email,
    int Type,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);

