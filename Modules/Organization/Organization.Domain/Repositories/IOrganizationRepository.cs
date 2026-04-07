using Organization.Domain.Entities;

namespace Organization.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<OrganizationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrganizationEntity> AddAsync(OrganizationEntity organization, CancellationToken cancellationToken = default);
    Task<OrganizationEntity> UpdateAsync(OrganizationEntity organization, CancellationToken cancellationToken = default);
}

