namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseExpiredIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid UnitId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime ExpiredAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
