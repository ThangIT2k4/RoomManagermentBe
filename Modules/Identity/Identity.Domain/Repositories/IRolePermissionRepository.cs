using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IRolePermissionRepository
{
    Task<RolePermissionEntity?> GetAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionEntity>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionEntity>> GetByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task<PagedResult<RolePermissionEntity>> GetByRoleIdPagedAsync(Guid roleId, int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<PagedResult<RolePermissionEntity>> GetByPermissionIdPagedAsync(Guid permissionId, int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<RolePermissionEntity> AddAsync(RolePermissionEntity rolePermission, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(RolePermissionEntity rolePermission, CancellationToken cancellationToken = default);
}
