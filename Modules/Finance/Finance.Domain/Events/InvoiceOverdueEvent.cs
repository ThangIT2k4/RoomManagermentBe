using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record InvoiceOverdueEvent(
    Guid InvoiceId,
    Guid OrganizationId,
    Guid LeaseId,
    string? InvoiceNo,
    DateOnly DueDate,
    decimal OutstandingAmount) : DomainEvent();
