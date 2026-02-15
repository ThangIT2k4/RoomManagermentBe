using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserProfileEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProfileEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<UserProfileEntity> AddAsync(UserProfileEntity profile, CancellationToken cancellationToken = default);
    Task<UserProfileEntity> UpdateAsync(UserProfileEntity profile, CancellationToken cancellationToken = default);
}
