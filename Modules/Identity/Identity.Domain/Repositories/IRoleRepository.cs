using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IRoleRepository
{
    Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<RoleEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<RoleEntity> AddAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default);
}
