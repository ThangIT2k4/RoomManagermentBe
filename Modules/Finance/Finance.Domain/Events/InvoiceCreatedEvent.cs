using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record InvoiceCreatedEvent(
    Guid InvoiceId,
    Guid OrganizationId,
    Guid LeaseId,
    string InvoiceNo,
    decimal TotalAmount,
    DateOnly DueDate,
    bool IsAutoCreated) : DomainEvent();
