using Integration.Domain.Entities;
using Integration.Domain.Enums;

namespace Integration.Domain.Repositories;

public interface IIntegrationWebhookEventRepository
{
    Task AddAsync(IntegrationWebhookEventEntity webhookEvent, CancellationToken cancellationToken);

    Task<IntegrationWebhookEventEntity?> GetByProviderAndExternalEventIdAsync(
        IntegrationProvider provider,
        string externalEventId,
        CancellationToken cancellationToken);

    Task UpdateAsync(IntegrationWebhookEventEntity webhookEvent, CancellationToken cancellationToken);
}
