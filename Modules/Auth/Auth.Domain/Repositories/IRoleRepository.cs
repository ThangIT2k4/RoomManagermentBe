using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Repositories;

public interface IRoleRepository
{
    Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleEntity?> GetByKeyCodeAsync(RoleKey keyCode, CancellationToken cancellationToken = default);
    Task<RoleEntity> AddAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task<PagedResult<RoleEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByKeyCodeAsync(
        RoleKey keyCode,
        Guid? excludeRoleId = null,
        CancellationToken cancellationToken = default);
}
