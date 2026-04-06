namespace RoomManagerment.Messaging.Contracts.Messages;

/// <summary>Triggered by scheduler/cron to run UC-F02-style auto invoice generation.</summary>
public record FinanceAutoInvoiceGenerationRequested
{
    public DateOnly RunDate { get; init; }
    public Guid CorrelationId { get; init; }
}

/// <summary>Triggered to mark sent invoices overdue and enqueue reminders (UC-F09).</summary>
public record FinanceOverdueSweepRequested
{
    public DateOnly AsOfDate { get; init; }
    public Guid CorrelationId { get; init; }
}
