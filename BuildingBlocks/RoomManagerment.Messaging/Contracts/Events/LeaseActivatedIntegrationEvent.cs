namespace RoomManagerment.Messaging.Contracts.Events;

public record LeaseActivatedIntegrationEvent
{
    public Guid LeaseId { get; init; }
    public Guid UnitId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public decimal RentAmount { get; init; }
    public decimal? DepositAmount { get; init; }
    public DateTime ActivatedAt { get; init; }
    public string SourceService { get; init; } = "Lease";
}
