using Auth.Domain.Common;

namespace Auth.Application.Services;

public interface IIntegrationEventPublisher
{
    ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

