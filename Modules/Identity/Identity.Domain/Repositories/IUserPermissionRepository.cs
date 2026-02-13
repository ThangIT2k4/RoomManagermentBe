using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IUserPermissionRepository
{
    Task<UserPermissionEntity?> GetAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermissionEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserPermissionEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserPermissionEntity> AddAsync(UserPermissionEntity userPermission, CancellationToken cancellationToken = default);
    Task<UserPermissionEntity> UpdateAsync(UserPermissionEntity userPermission, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(UserPermissionEntity userPermission, CancellationToken cancellationToken = default);
}
