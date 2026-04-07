using Organization.Application.Dtos;

namespace Organization.Application.Services;

public interface IOrganizationCacheService
{
    Task<OrganizationDto?> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task SetOrganizationAsync(OrganizationDto organization, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task RemoveOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
