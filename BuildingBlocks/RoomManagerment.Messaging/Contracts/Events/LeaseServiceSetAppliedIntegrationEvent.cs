namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseServiceSetAppliedIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid UnitId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseServiceSetId { get; init; }
    public DateTime AppliedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
