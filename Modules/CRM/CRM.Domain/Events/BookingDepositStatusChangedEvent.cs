using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record BookingDepositStatusChangedEvent(
    Guid BookingDepositId,
    string PreviousStatus,
    string CurrentStatus,
    DateTimeOffset OccurredOn) : DomainEvent;
