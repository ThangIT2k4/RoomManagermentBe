using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record InvoicePaymentRecordedEvent(
    Guid InvoiceId,
    Guid OrganizationId,
    Guid PaymentId,
    decimal Amount,
    decimal NewPaidAmount,
    decimal TotalAmount,
    bool IsFullyPaid,
    string? ReferenceCode) : DomainEvent();
