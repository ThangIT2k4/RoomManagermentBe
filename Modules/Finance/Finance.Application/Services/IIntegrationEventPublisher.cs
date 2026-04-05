using Finance.Domain.Common;

namespace Finance.Application.Services;

public interface IFinanceIntegrationEventPublisher
{
    ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
