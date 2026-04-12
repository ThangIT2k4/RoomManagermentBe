using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.SearchInvoices;

public sealed record SearchInvoicesQuery(
    Guid OrganizationId,
    IReadOnlyList<string>? Statuses = null,
    Guid? LeaseId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string? Search = null,
    int Page = 1,
    int PerPage = 20)
    : IAppRequest<Result<PagedInvoicesDto>>;

public sealed record ListTenantInvoicesQuery(Guid TenantUserId)
    : IAppRequest<Result<IReadOnlyList<InvoiceDto>>>;
