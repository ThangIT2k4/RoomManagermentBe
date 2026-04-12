
namespace Finance.Application.Services;

public sealed record OnlinePaymentInitiationResult(string Status, string? Message, IReadOnlyDictionary<string, string>? Payload);

public interface IOnlinePaymentInitiator
{
    Task<Result<OnlinePaymentInitiationResult>> InitiateAsync(
        Guid invoiceId,
        Guid tenantUserId,
        string methodKey,
        CancellationToken cancellationToken = default);
}
