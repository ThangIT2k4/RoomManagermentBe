using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.CreateInvoice;

public sealed class CreateManualInvoiceCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<CreateManualInvoiceCommand, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(CreateManualInvoiceCommand request, CancellationToken cancellationToken)
        => finance.CreateManualInvoiceAsync(
            request.OrganizationId,
            request.UserId,
            request.LeaseId,
            request.InvoiceDate,
            request.DueDate,
            request.Notes,
            request.Items,
            cancellationToken);
}
