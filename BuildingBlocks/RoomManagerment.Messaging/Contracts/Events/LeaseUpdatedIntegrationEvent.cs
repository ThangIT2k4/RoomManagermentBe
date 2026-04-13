namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseUpdatedIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid UnitId { get; init; }
    public string[] ChangedFields { get; init; } = [];
    public DateTime UpdatedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
