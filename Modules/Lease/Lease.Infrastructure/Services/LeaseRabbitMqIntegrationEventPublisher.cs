using System.Threading.Channels;
using Lease.Application.Services;
using Lease.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Lease.Infrastructure.Services;

public sealed class LeaseRabbitMqIntegrationEventPublisher : ILeaseIntegrationEventPublisher
{
    private readonly Channel<IntegrationEventEnvelope> _channel = Channel.CreateUnbounded<IntegrationEventEnvelope>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    private readonly ILogger<LeaseRabbitMqIntegrationEventPublisher> _logger;

    public LeaseRabbitMqIntegrationEventPublisher(ILogger<LeaseRabbitMqIntegrationEventPublisher> logger)
    {
        _logger = logger;
    }

    public ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (!LeaseIntegrationEventMapper.TryMap(domainEvent, out var envelope) || envelope is null)
        {
            return ValueTask.CompletedTask;
        }

        if (!_channel.Writer.TryWrite(envelope))
        {
            _logger.LogWarning("Không thể đưa lease integration event {EventType} vào hàng đợi.", domainEvent.GetType().Name);
        }

        return ValueTask.CompletedTask;
    }

    internal ChannelReader<IntegrationEventEnvelope> Reader => _channel.Reader;
}
