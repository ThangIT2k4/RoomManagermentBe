using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.InitiateOnlinePayment;

public sealed class InitiateOnlinePaymentCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<InitiateOnlinePaymentCommand, Result<OnlinePaymentInitiationResult>>
{
    public Task<Result<OnlinePaymentInitiationResult>> Handle(InitiateOnlinePaymentCommand request, CancellationToken cancellationToken)
        => finance.InitiateOnlinePaymentAsync(request.TenantUserId, request.InvoiceId, request.MethodKey, cancellationToken);
}
