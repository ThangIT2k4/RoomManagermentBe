using Organization.Domain.Entities;

namespace Organization.Domain.Repositories;

public interface IOrganizationUserCapabilityRepository
{
    Task<OrganizationUserCapabilityEntity?> GetByOrganizationUserAndCapabilityAsync(Guid organizationUserId, Guid capabilityId, CancellationToken cancellationToken = default);
    Task<OrganizationUserCapabilityEntity> UpsertAsync(OrganizationUserCapabilityEntity entity, CancellationToken cancellationToken = default);
}
