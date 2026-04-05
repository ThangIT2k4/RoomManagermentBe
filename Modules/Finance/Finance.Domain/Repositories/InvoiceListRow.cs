using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public sealed record InvoiceListRow(InvoiceEntity Invoice, string? LeaseNo, string? TenantName);
