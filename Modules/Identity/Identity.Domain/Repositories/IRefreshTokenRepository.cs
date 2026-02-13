using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshTokenEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<RefreshTokenEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshTokenEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<RefreshTokenEntity> AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshTokenEntity> UpdateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
}
