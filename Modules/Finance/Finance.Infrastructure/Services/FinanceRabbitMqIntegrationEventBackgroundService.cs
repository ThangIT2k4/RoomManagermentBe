using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services;

public sealed class FinanceRabbitMqIntegrationEventBackgroundService : BackgroundService
{
    private readonly FinanceRabbitMqIntegrationEventPublisher _publisher;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<FinanceRabbitMqIntegrationEventBackgroundService> _logger;

    public FinanceRabbitMqIntegrationEventBackgroundService(
        FinanceRabbitMqIntegrationEventPublisher publisher,
        IPublishEndpoint publishEndpoint,
        ILogger<FinanceRabbitMqIntegrationEventBackgroundService> logger)
    {
        _publisher = publisher;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in _publisher.Reader.ReadAllAsync(stoppingToken))
        {
            await PublishWithRetryAsync(envelope, stoppingToken);
        }
    }

    private async Task PublishWithRetryAsync(IntegrationEventEnvelope envelope, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _publishEndpoint.Publish(envelope.Message, envelope.Message.GetType(), stoppingToken);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Finance RabbitMQ publish failed for {RoutingKey}. Retrying...", envelope.RoutingKey);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
