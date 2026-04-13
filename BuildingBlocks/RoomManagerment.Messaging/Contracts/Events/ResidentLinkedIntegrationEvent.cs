namespace RoomManagerment.Messaging.Contracts.Events;

public record ResidentLinkedIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid ResidentId { get; init; }
    public Guid UserId { get; init; }
    public DateTime LinkedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
