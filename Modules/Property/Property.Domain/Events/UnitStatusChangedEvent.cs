using Property.Domain.Common;

namespace Property.Domain.Events;

public sealed record UnitStatusChangedEvent(Guid UnitId, short OldStatus, short NewStatus, DateTimeOffset OccurredOn)
    : DomainEvent(OccurredOn);
