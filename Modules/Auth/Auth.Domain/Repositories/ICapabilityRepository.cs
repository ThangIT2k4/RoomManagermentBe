using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Repositories;

public interface ICapabilityRepository
{
    Task<CapabilityEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CapabilityEntity?> GetByKeyCodeAsync(CapabilityKey keyCode, CancellationToken cancellationToken = default);
    Task<CapabilityEntity> AddAsync(CapabilityEntity capability, CancellationToken cancellationToken = default);
    Task<CapabilityEntity> UpdateAsync(CapabilityEntity capability, CancellationToken cancellationToken = default);
    Task<PagedResult<CapabilityEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByKeyCodeAsync(
        CapabilityKey keyCode,
        Guid? excludeCapabilityId = null,
        CancellationToken cancellationToken = default);
}
