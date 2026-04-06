using CRM.Domain.Common;

namespace CRM.Application.Services;

public interface IIntegrationEventPublisher
{
    ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
