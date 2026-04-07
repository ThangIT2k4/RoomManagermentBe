using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record ViewingStatusChangedEvent(
    Guid ViewingId,
    string PreviousStatus,
    string CurrentStatus,
    DateTimeOffset OccurredOn) : DomainEvent;
