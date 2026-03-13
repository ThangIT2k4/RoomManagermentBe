using Integration.Domain.Entities;

namespace Integration.Domain.Repositories;

public interface IIntegrationCredentialRepository
{
    Task AddAsync(IntegrationCredentialEntity credential, CancellationToken cancellationToken);

    Task<IntegrationCredentialEntity?> GetByConnectionIdAsync(Guid connectionId, CancellationToken cancellationToken);

    Task UpdateAsync(IntegrationCredentialEntity credential, CancellationToken cancellationToken);
}
