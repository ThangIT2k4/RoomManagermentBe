using Integration.Domain.Entities;

namespace Integration.Domain.Repositories;

public interface IIntegrationConnectionRepository
{
    Task AddAsync(IntegrationConnectionEntity connection, CancellationToken cancellationToken);

    Task<IntegrationConnectionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IntegrationConnectionEntity>> GetByTenantAndUserAsync(
        string tenantId,
        string userId,
        CancellationToken cancellationToken);

    Task UpdateAsync(IntegrationConnectionEntity connection, CancellationToken cancellationToken);
}
