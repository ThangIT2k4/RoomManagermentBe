using System.Threading.Channels;
using Finance.Application.Services;
using Finance.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services;

public sealed class FinanceRabbitMqIntegrationEventPublisher : IFinanceIntegrationEventPublisher
{
    private readonly Channel<IntegrationEventEnvelope> _channel = Channel.CreateUnbounded<IntegrationEventEnvelope>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    private readonly ILogger<FinanceRabbitMqIntegrationEventPublisher> _logger;

    public FinanceRabbitMqIntegrationEventPublisher(ILogger<FinanceRabbitMqIntegrationEventPublisher> logger)
    {
        _logger = logger;
    }

    public ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (!FinanceIntegrationEventMapper.TryMap(domainEvent, out var envelope) || envelope is null)
        {
            return ValueTask.CompletedTask;
        }

        if (!_channel.Writer.TryWrite(envelope))
        {
            _logger.LogWarning("Unable to queue finance integration event {EventType}.", domainEvent.GetType().Name);
        }

        return ValueTask.CompletedTask;
    }

    internal ChannelReader<IntegrationEventEnvelope> Reader => _channel.Reader;
}
