namespace Organization.Application.Services;

public interface IIntegrationEventPublisher
{
    ValueTask EnqueueAsync(object message, string routingKey, CancellationToken cancellationToken = default);
}
