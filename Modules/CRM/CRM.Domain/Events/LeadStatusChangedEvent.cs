using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record LeadStatusChangedEvent(
    Guid LeadId,
    string PreviousStatus,
    string CurrentStatus,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
