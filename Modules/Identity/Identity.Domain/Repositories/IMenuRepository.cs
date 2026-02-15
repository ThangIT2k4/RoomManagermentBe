using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IMenuRepository
{
    Task<MenuEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuEntity>> GetByParentIdAsync(Guid? parentId, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<MenuEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<MenuEntity> AddAsync(MenuEntity menu, CancellationToken cancellationToken = default);
    Task<MenuEntity> UpdateAsync(MenuEntity menu, CancellationToken cancellationToken = default);
}
