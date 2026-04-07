using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Organization.Application.Services;

namespace Organization.Infrastructure.Services;

public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly Channel<IntegrationEventEnvelope> _channel = Channel.CreateUnbounded<IntegrationEventEnvelope>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
    private readonly ILogger<RabbitMqIntegrationEventPublisher> _logger;

    public RabbitMqIntegrationEventPublisher(ILogger<RabbitMqIntegrationEventPublisher> logger)
    {
        _logger = logger;
    }

    public ValueTask EnqueueAsync(object message, string routingKey, CancellationToken cancellationToken = default)
    {
        if (!_channel.Writer.TryWrite(new IntegrationEventEnvelope(routingKey, message)))
        {
            _logger.LogWarning("Unable to queue integration event {RoutingKey}.", routingKey);
        }

        return ValueTask.CompletedTask;
    }

    internal ChannelReader<IntegrationEventEnvelope> Reader => _channel.Reader;
}
