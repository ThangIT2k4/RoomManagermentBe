using Finance.Application.Services;

namespace Finance.Infrastructure.Services;

public sealed class NoOpOnlinePaymentInitiator : IOnlinePaymentInitiator
{
    public Task<Result<OnlinePaymentInitiationResult>> InitiateAsync(
        Guid invoiceId,
        Guid tenantUserId,
        string methodKey,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Result<OnlinePaymentInitiationResult>.Failure(
                new Error(
                    "Finance.Online.NotImplemented",
                    "Online payment (SePay/VNPay/MoMo) is not wired yet. Integrate IOnlinePaymentInitiator in Infrastructure.")));
    }
}
