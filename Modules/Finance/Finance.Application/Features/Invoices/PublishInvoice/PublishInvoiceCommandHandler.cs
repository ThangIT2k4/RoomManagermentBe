using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.PublishInvoice;

public sealed class PublishInvoiceCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<PublishInvoiceCommand, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(PublishInvoiceCommand request, CancellationToken cancellationToken)
        => finance.PublishInvoiceAsync(request.OrganizationId, request.InvoiceId, cancellationToken);
}
