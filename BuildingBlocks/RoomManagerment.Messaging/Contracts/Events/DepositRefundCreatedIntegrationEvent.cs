namespace RoomManagerment.Messaging.Contracts.Events;

public record DepositRefundCreatedIntegrationEvent
{
    public Guid RefundId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public Guid? TenantId { get; init; }
    public decimal Amount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
