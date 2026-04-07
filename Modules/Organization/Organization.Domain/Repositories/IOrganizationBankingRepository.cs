using Organization.Domain.Entities;

namespace Organization.Domain.Repositories;

public interface IOrganizationBankingRepository
{
    Task<OrganizationBankingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationBankingEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<OrganizationBankingEntity> AddAsync(OrganizationBankingEntity entity, CancellationToken cancellationToken = default);
    Task<OrganizationBankingEntity> UpdateAsync(OrganizationBankingEntity entity, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, Guid deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default);
}
