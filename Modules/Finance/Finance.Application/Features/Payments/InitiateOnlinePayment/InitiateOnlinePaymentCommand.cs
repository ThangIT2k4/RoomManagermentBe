using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.InitiateOnlinePayment;

public sealed record InitiateOnlinePaymentCommand(Guid TenantUserId, Guid InvoiceId, string MethodKey)
    : IAppRequest<Result<OnlinePaymentInitiationResult>>;
