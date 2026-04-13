namespace RoomManagerment.Messaging.Contracts.Events;

public record DepositRefundForfeitedIntegrationEvent
{
    public Guid RefundId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public Guid? TenantId { get; init; }
    public string Reason { get; init; } = default!;
    public DateTime ForfeitedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
