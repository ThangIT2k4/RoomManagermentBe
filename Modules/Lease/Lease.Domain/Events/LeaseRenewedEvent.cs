using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseRenewedEvent(
    Guid OldLeaseId,
    Guid NewLeaseId,
    Guid UnitId,
    decimal RentAmount,
    DateOnly StartDate,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
