using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record InvoiceCancelledEvent(
    Guid InvoiceId,
    Guid OrganizationId,
    Guid LeaseId,
    string? InvoiceNo,
    string? Reason) : DomainEvent();
