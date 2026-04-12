using Finance.Application.Services;

namespace Finance.Infrastructure.Services;

/// <summary>No-op webhook: returns success so gateways are not retried until SePay is implemented.</summary>
public sealed class NoOpPaymentWebhookHandler : IPaymentWebhookHandler
{
    public Task<Result> HandleAsync(
        string rawBody,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success());
}
