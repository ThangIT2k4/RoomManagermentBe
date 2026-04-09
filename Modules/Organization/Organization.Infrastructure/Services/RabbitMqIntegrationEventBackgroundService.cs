using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Organization.Infrastructure.Services;

public sealed class RabbitMqIntegrationEventBackgroundService : BackgroundService
{
    private readonly RabbitMqIntegrationEventPublisher _publisher;
    private readonly IBus _bus;
    private readonly ILogger<RabbitMqIntegrationEventBackgroundService> _logger;

    public RabbitMqIntegrationEventBackgroundService(
        RabbitMqIntegrationEventPublisher publisher,
        IBus bus,
        ILogger<RabbitMqIntegrationEventBackgroundService> logger)
    {
        _publisher = publisher;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in _publisher.Reader.ReadAllAsync(stoppingToken))
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _bus.Publish(envelope.Message, envelope.Message.GetType(), stoppingToken);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ publish failed for {RoutingKey}. Retrying...", envelope.RoutingKey);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}
