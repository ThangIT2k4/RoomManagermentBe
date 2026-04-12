using Finance.Application.Dtos;

namespace Finance.Application.Features.Invoices;

public sealed record InvoiceListItemDto(
    Guid Id,
    Guid OrganizationId,
    Guid? LeaseId,
    string? InvoiceNo,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string Status,
    decimal TotalAmount,
    decimal PaidAmount,
    string? LeaseNo,
    string? TenantName,
    DateTime CreatedAt);

public sealed record PagedInvoicesResult(
    IReadOnlyList<InvoiceListItemDto> Items,
    int TotalCount,
    int Page,
    int PerPage);

public sealed record InvoiceItemLineRequest(
    string ItemType,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    Guid? ServiceId,
    Guid? MeterReadingId,
    Guid? TicketLogId);
