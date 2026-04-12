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
                    "Thanh toán online (SePay/VNPay/MoMo) chưa được cấu hình. Hãy tích hợp IOnlinePaymentInitiator trong lớp Infrastructure.")));
    }
}
