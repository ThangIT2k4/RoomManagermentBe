using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseExpiredEvent(
    Guid LeaseId,
    Guid UnitId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
