using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record CommissionEventStatusChangedEvent(
    Guid CommissionEventId,
    Guid OrganizationId,
    string PreviousStatus,
    string CurrentStatus,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
