using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IUserRoleRepository
{
    Task<UserRoleEntity?> GetAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRoleEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRoleEntity>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserRoleEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<UserRoleEntity>> GetByRoleIdPagedAsync(Guid roleId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserRoleEntity> AddAsync(UserRoleEntity userRole, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(UserRoleEntity userRole, CancellationToken cancellationToken = default);
}
