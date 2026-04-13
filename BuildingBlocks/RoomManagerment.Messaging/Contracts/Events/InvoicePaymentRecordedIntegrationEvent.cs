namespace RoomManagerment.Messaging.Contracts.Events;

public record InvoicePaymentRecordedIntegrationEvent
{
    public Guid InvoiceId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public decimal NewPaidAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsFullyPaid { get; init; }
    public string? ReferenceCode { get; init; }
    public DateTime RecordedAt { get; init; }
    public string SourceService { get; init; } = "Finance";
}
