using Property.Domain.Common;

namespace Property.Domain.Events;

public sealed record TicketStatusChangedEvent(Guid TicketId, string OldStatus, string NewStatus, DateTimeOffset OccurredOn)
    : DomainEvent(OccurredOn);
