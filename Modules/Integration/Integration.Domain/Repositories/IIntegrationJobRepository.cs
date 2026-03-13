using Integration.Domain.Entities;

namespace Integration.Domain.Repositories;

public interface IIntegrationJobRepository
{
    Task AddAsync(IntegrationJobEntity job, CancellationToken cancellationToken);

    Task<IntegrationJobEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(IntegrationJobEntity job, CancellationToken cancellationToken);
}
