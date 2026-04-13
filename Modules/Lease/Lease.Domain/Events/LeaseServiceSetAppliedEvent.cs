using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseServiceSetAppliedEvent(
    Guid LeaseId,
    Guid UnitId,
    Guid OrganizationId,
    Guid LeaseServiceSetId,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
