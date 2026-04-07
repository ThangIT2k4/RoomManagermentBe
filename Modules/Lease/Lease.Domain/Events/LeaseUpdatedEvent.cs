using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseUpdatedEvent(
    Guid LeaseId,
    Guid UnitId,
    string[] ChangedFields,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
