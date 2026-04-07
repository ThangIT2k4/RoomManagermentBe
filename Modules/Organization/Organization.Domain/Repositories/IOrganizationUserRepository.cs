using Organization.Domain.Entities;

namespace Organization.Domain.Repositories;

public interface IOrganizationUserRepository
{
    Task<OrganizationUserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrganizationUserEntity?> GetByOrgAndUserAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    Task<OrganizationUserEntity?> GetByInvitationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationUserEntity>> GetPagedByOrgAsync(Guid organizationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<long> CountByOrgAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<OrganizationUserEntity> AddAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default);
    Task<OrganizationUserEntity> UpdateAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default);
}
