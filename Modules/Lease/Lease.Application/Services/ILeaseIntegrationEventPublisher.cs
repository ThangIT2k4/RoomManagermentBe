using Lease.Domain.Common;

namespace Lease.Application.Services;

public interface ILeaseIntegrationEventPublisher
{
    ValueTask PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
