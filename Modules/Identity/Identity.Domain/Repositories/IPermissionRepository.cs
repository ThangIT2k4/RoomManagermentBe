using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IPermissionRepository
{
    Task<PermissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PermissionEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<PermissionEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<PermissionEntity> AddAsync(PermissionEntity permission, CancellationToken cancellationToken = default);
    Task<PermissionEntity> UpdateAsync(PermissionEntity permission, CancellationToken cancellationToken = default);
}
