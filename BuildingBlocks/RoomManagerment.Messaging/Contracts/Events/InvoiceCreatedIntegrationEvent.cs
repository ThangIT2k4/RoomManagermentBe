namespace RoomManagerment.Messaging.Contracts.Events;

public record InvoiceCreatedIntegrationEvent
{
    public Guid InvoiceId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid LeaseId { get; init; }
    public string InvoiceNo { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public DateOnly DueDate { get; init; }
    public bool IsAutoCreated { get; init; }
    public DateTime CreatedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
