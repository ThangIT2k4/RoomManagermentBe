using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.SearchInvoices;

public sealed class SearchInvoicesQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<SearchInvoicesQuery, Result<PagedInvoicesDto>>
{
    public Task<Result<PagedInvoicesDto>> Handle(SearchInvoicesQuery request, CancellationToken cancellationToken)
        => finance.SearchInvoicesAsync(
            request.OrganizationId,
            request.Statuses,
            request.LeaseId,
            request.FromDate,
            request.ToDate,
            request.Search,
            request.Page,
            request.PerPage,
            cancellationToken);
}

public sealed class ListTenantInvoicesQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<ListTenantInvoicesQuery, Result<IReadOnlyList<InvoiceDto>>>
{
    public Task<Result<IReadOnlyList<InvoiceDto>>> Handle(ListTenantInvoicesQuery request, CancellationToken cancellationToken)
        => finance.ListTenantInvoicesAsync(request.TenantUserId, cancellationToken);
}
