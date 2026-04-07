using Lease.Domain.Common;

namespace Lease.Domain.Events;

public sealed record LeaseActivatedEvent(
    Guid LeaseId,
    Guid UnitId,
    Guid OrganizationId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RentAmount,
    decimal? DepositAmount,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
