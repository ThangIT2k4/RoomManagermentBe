
namespace Finance.Application.Services;

public interface IPaymentWebhookHandler
{
    Task<Result> HandleAsync(string rawBody, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken = default);
}
