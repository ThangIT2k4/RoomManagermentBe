namespace RoomManagerment.Messaging.Contracts.Events;

public record InvoiceCancelledIntegrationEvent
{
    public Guid InvoiceId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public string? InvoiceNo { get; init; }
    public string? Reason { get; init; }
    public DateTime CancelledAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
