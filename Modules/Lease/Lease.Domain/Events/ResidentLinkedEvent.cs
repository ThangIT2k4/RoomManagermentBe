using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record ResidentLinkedEvent(
    Guid LeaseId,
    Guid ResidentId,
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
