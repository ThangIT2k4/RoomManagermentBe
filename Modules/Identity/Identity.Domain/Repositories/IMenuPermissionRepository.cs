using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IMenuPermissionRepository
{
    Task<MenuPermissionEntity?> GetAsync(Guid menuId, string permissionCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuPermissionEntity>> GetByMenuIdAsync(Guid menuId, CancellationToken cancellationToken = default);
    Task<PagedResult<MenuPermissionEntity>> GetByMenuIdPagedAsync(Guid menuId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<MenuPermissionEntity> AddAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default);
}
