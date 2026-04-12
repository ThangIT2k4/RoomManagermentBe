using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.GetInvoice;

public sealed class GetInvoiceQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<GetInvoiceQuery, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        => finance.GetInvoiceAsync(request.OrganizationId, request.InvoiceId, cancellationToken);
}

public sealed class GetInvoiceForTenantQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<GetInvoiceForTenantQuery, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(GetInvoiceForTenantQuery request, CancellationToken cancellationToken)
        => finance.GetInvoiceForTenantAsync(request.TenantUserId, request.InvoiceId, cancellationToken);
}
