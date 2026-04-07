using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseTerminatedEvent(
    Guid LeaseId,
    Guid UnitId,
    Guid OrganizationId,
    DateOnly TerminationDate,
    string Reason,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
