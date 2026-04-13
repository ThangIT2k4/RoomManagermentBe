namespace RoomManagerment.Messaging.Contracts.Events;

public record InvoiceOverdueIntegrationEvent
{
    public Guid InvoiceId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public string? InvoiceNo { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime MarkedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
