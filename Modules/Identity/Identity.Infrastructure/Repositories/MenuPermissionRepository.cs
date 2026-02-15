using Identity.Domain.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Infrastructure.Repositories;

public sealed class MenuPermissionRepository : IMenuPermissionRepository
{
    public Task<MenuPermissionEntity?> GetAsync(Guid menuId, string permissionCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<MenuPermissionEntity>> GetByMenuIdAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<MenuPermissionEntity>> GetByMenuIdPagedAsync(Guid menuId, int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<MenuPermissionEntity> AddAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}