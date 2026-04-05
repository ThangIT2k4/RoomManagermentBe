using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public sealed record PaymentListRow(
    PaymentEntity Payment,
    string? InvoiceNo,
    decimal InvoiceTotal,
    string? LeaseNo,
    string? TenantName,
    string? MethodName);
