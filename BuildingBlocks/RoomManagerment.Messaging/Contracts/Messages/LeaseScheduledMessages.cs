namespace RoomManagerment.Messaging.Contracts.Messages;

public record LeaseExpiringCheckRequested
{
    public DateOnly AsOfDate { get; init; }
    public Guid CorrelationId { get; init; }
}

public record LeaseExpirySweepRequested
{
    public DateOnly AsOfDate { get; init; }
    public Guid CorrelationId { get; init; }
}
