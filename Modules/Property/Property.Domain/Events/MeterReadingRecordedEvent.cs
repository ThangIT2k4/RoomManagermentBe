using Property.Domain.Common;

namespace Property.Domain.Events;

public sealed record MeterReadingRecordedEvent(Guid MeterReadingId, Guid MeterId, decimal Value, DateTimeOffset OccurredOn)
    : DomainEvent(OccurredOn);
