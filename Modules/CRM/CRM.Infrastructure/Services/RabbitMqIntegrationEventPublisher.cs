using System.Threading.Channels;
using CRM.Application.Services;
using CRM.Domain.Common;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Services;

public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly Channel<IntegrationEventEnvelope> _channel = Channel.CreateUnbounded<IntegrationEventEnvelope>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    private readonly ILogger<RabbitMqIntegrationEventPublisher> _logger;

    public RabbitMqIntegrationEventPublisher(ILogger<RabbitMqIntegrationEventPublisher> logger)
    {
        _logger = logger;
    }

    public ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (!CrmIntegrationEventMapper.TryMap(domainEvent, out var envelope) || envelope is null)
        {
            return ValueTask.CompletedTask;
        }

        if (!_channel.Writer.TryWrite(envelope))
        {
            _logger.LogWarning("Không thể đưa CRM integration event {EventType} vào hàng đợi.", domainEvent.GetType().Name);
        }

        return ValueTask.CompletedTask;
    }

    internal ChannelReader<IntegrationEventEnvelope> Reader => _channel.Reader;
}
