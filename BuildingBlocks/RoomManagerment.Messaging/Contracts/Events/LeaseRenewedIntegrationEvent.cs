namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseRenewedIntegrationEvent
{
    public Guid OldLeaseId { get; init; }
    public Guid NewLeaseId { get; init; }
    public Guid UnitId { get; init; }
    public decimal RentAmount { get; init; }
    public DateOnly StartDate { get; init; }
    public DateTime RenewedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
