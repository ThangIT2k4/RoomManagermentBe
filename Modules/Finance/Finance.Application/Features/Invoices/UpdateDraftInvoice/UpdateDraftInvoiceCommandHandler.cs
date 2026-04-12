using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.UpdateDraftInvoice;

public sealed class UpdateDraftInvoiceCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<UpdateDraftInvoiceCommand, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(UpdateDraftInvoiceCommand request, CancellationToken cancellationToken)
        => finance.UpdateDraftInvoiceAsync(
            request.OrganizationId,
            request.UserId,
            request.InvoiceId,
            request.DueDate,
            request.Notes,
            request.Items,
            cancellationToken);
}
