using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.RecordPayment;

public sealed class RecordManualPaymentCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<RecordManualPaymentCommand, Result<InvoiceDto>>
{
    public Task<Result<InvoiceDto>> Handle(RecordManualPaymentCommand request, CancellationToken cancellationToken)
        => finance.RecordManualPaymentAsync(
            new RecordManualPaymentRequest(
                request.OrganizationId,
                request.UserId,
                request.InvoiceId,
                request.MethodId,
                request.Amount,
                request.PaidAtUtc,
                request.ReferenceCode,
                request.Note),
            cancellationToken);
}
