using Finance.Application.Services;
using Finance.Domain.Common;

namespace Finance.Infrastructure.Services;

public sealed class NoOpFinanceIntegrationEventPublisher : IFinanceIntegrationEventPublisher
{
    public ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
}
