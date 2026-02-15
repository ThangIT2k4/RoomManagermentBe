using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IUserMenuOverrideRepository
{
    Task<UserMenuOverrideEntity?> GetAsync(Guid userId, Guid menuId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserMenuOverrideEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserMenuOverrideEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<UserMenuOverrideEntity> AddAsync(UserMenuOverrideEntity userMenuOverride, CancellationToken cancellationToken = default);
    Task<UserMenuOverrideEntity> UpdateAsync(UserMenuOverrideEntity userMenuOverride, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(UserMenuOverrideEntity userMenuOverride, CancellationToken cancellationToken = default);
}
