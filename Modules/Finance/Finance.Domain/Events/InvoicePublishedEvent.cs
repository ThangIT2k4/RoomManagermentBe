using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record InvoicePublishedEvent(
    Guid InvoiceId,
    Guid OrganizationId,
    Guid LeaseId,
    string InvoiceNo,
    decimal TotalAmount,
    DateOnly DueDate) : DomainEvent();
