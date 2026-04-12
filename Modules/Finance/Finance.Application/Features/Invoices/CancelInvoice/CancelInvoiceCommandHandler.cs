using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.CancelInvoice;

public sealed class CancelInvoiceCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<CancelInvoiceCommand, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        => finance.CancelInvoiceAsync(request.OrganizationId, request.UserId, request.InvoiceId, request.Reason, cancellationToken);
}
