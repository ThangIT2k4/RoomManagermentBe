using Integration.Application.Common;
using Integration.Application.Services;
using Integration.Domain.Entities;
using Integration.Domain.Providers;
using Integration.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Integration.Application.Features.Webhooks.HandleWebhook;

public sealed class HandleWebhookCommandHandler(
    IIntegrationProviderClientFactory providerClientFactory,
    IIntegrationWebhookEventRepository webhookEventRepository,
    ILogger<HandleWebhookCommandHandler> logger)
    : IRequestHandler<HandleWebhookCommand, Result>
{
    public async Task<Result> Handle(HandleWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var providerClient = providerClientFactory.GetClient(request.Provider);
            var providerResult = await providerClient.HandleWebhookAsync(
                new ProviderWebhookRequest(
                    request.Signature,
                    request.PayloadJson,
                    request.Headers),
                cancellationToken);

            if (!providerResult.IsVerified)
            {
                logger.LogWarning("Webhook signature verification failed for provider {Provider}", request.Provider);
                return Result.Failure(new Error("WEBHOOK_NOT_VERIFIED", "Webhook signature is invalid."));
            }

            var existingEvent = await webhookEventRepository.GetByProviderAndExternalEventIdAsync(
                request.Provider,
                providerResult.ExternalEventId,
                cancellationToken);

            if (existingEvent is not null)
            {
                return Result.Success();
            }

            var webhookEvent = IntegrationWebhookEventEntity.Create(
                request.Provider,
                providerResult.ExternalEventId,
                request.Signature,
                request.PayloadJson);

            webhookEvent.MarkProcessed();

            await webhookEventRepository.AddAsync(webhookEvent, cancellationToken);
            await webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle webhook for provider {Provider}", request.Provider);
            return Result.Failure(new Error("WEBHOOK_HANDLE_FAILED", "Unable to process webhook."));
        }
    }
}
