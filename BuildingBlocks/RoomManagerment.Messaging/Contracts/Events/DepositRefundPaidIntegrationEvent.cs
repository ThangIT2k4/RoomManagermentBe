namespace RoomManagerment.Messaging.Contracts.Events;

public record DepositRefundPaidIntegrationEvent
{
    public Guid RefundId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public Guid? TenantId { get; init; }
    public decimal Amount { get; init; }
    public DateTime PaidAt { get; init; }
    public DateTime EventCreatedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
