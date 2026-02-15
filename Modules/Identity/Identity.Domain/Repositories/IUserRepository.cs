using Identity.Domain.Common;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<UserEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
}
