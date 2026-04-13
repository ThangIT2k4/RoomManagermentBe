namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseTerminatedIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid UnitId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateOnly TerminationDate { get; init; }
    public string Reason { get; init; } = default!;
    public DateTime TerminatedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
