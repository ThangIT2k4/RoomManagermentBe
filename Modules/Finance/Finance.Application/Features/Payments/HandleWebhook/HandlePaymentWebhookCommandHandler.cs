using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.HandleWebhook;

public sealed class HandlePaymentWebhookCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<HandlePaymentWebhookCommand, Result>
{
    public Task<Result> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
        => finance.HandlePaymentWebhookAsync(request.RawBody, request.Headers, cancellationToken);
}
