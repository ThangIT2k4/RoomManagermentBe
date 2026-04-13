using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lease.Infrastructure.Services;

public sealed class LeaseRabbitMqIntegrationEventBackgroundService : BackgroundService
{
    private readonly LeaseRabbitMqIntegrationEventPublisher _publisher;
    private readonly IBus _bus;
    private readonly ILogger<LeaseRabbitMqIntegrationEventBackgroundService> _logger;

    public LeaseRabbitMqIntegrationEventBackgroundService(
        LeaseRabbitMqIntegrationEventPublisher publisher,
        IBus bus,
        ILogger<LeaseRabbitMqIntegrationEventBackgroundService> logger)
    {
        _publisher = publisher;
        _bus = bus;
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
                await _bus.Publish(envelope.Message, envelope.Message.GetType(), stoppingToken);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lease RabbitMQ publish thất bại cho {RoutingKey}. Đang thử lại...", envelope.RoutingKey);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
