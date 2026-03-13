using Integration.Domain.Enums;

namespace Integration.Domain.Providers;

public interface IIntegrationProviderClient
{
    IntegrationProvider Provider { get; }

    Task<ProviderConnectResult> ConnectAsync(ProviderConnectRequest request, CancellationToken cancellationToken);

    Task DisconnectAsync(ProviderDisconnectRequest request, CancellationToken cancellationToken);

    Task<ProviderActionResult> ExecuteActionAsync(ProviderActionRequest request, CancellationToken cancellationToken);

    Task<ProviderWebhookResult> HandleWebhookAsync(ProviderWebhookRequest request, CancellationToken cancellationToken);
}
