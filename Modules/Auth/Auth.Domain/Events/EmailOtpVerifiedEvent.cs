using Auth.Domain.Common;

namespace Auth.Domain.Events;

public sealed record EmailOtpVerifiedEvent(
    Guid EmailOtpId,
    Guid? UserId,
    string Email,
    int Type,
    DateTimeOffset VerifiedAt) : DomainEvent(VerifiedAt);

