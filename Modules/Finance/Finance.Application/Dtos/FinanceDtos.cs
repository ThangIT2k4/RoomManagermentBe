namespace Finance.Application.Dtos;

public sealed record InvoiceItemLineDto(
    string ItemType,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    Guid? ServiceId,
    Guid? MeterReadingId,
    Guid? TicketLogId);

public sealed record InvoiceItemDto(
    Guid Id,
    string ItemType,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Amount,
    Guid? ServiceId,
    Guid? MeterReadingId,
    Guid? TicketLogId);

public sealed record InvoiceDto(
    Guid Id,
    Guid OrganizationId,
    Guid? LeaseId,
    bool IsAutoCreated,
    string? InvoiceNo,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string Status,
    decimal TotalAmount,
    decimal PaidAmount,
    string? Notes,
    Guid? CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? LeaseNo,
    string? TenantName,
    IReadOnlyList<InvoiceItemDto> Items,
    IReadOnlyList<PaymentDto> Payments);

public sealed record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    Guid? MethodId,
    decimal Amount,
    DateTime? PaidAt,
    string? ReferenceCode,
    string Status,
    string? MethodName);

public sealed record PagedInvoicesDto(
    IReadOnlyList<InvoiceDto> Items,
    int TotalCount,
    int Page,
    int PerPage);

public sealed record PagedPaymentsDto(
    IReadOnlyList<PaymentDto> Items,
    int TotalCount,
    int Page,
    int PerPage);

public sealed record RecordManualPaymentRequest(
    Guid OrganizationId,
    Guid UserId,
    Guid InvoiceId,
    Guid MethodId,
    decimal Amount,
    DateTime PaidAtUtc,
    string? ReferenceCode,
    string? Note);

public sealed record DepositRefundDto(
    Guid Id,
    Guid LeaseId,
    Guid OrganizationId,
    Guid? TenantId,
    decimal Amount,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? PaidAt);

public sealed record CreateDepositRefundRequest(
    Guid OrganizationId,
    Guid UserId,
    Guid LeaseId,
    decimal Amount,
    string? Notes);

public sealed record ConfirmDepositRefundRequest(Guid OrganizationId, Guid UserId, DateTime PaidAtUtc, string? ReferenceCode);

public sealed record ForfeitDepositRefundRequest(Guid OrganizationId, Guid UserId, string Reason);

public sealed record RevenueMonthDto(DateOnly Month, decimal TotalRevenue, int InvoiceCount, int LeaseCount);

public sealed record DebtSummaryDto(
    int OverdueCount,
    decimal OverdueAmount,
    int DueSoonCount,
    decimal DueSoonAmount);
