using Auth.Domain.Common;
using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface ISessionRepository
{
    Task<SessionEntity?> GetByIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<SessionEntity> AddAsync(SessionEntity session, CancellationToken cancellationToken = default);
    Task<SessionEntity> UpdateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    Task DeleteAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<PagedResult<SessionEntity>> GetByUserPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        bool includeExpired = false,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SessionEntity>> GetActiveByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<long> DeleteExpiredAsync(
        DateTimeOffset now,
        CancellationToken cancellationToken = default);
    Task<long> DeleteAllByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<long> DeleteAllByUserExceptAsync(
        Guid userId,
        string exceptSessionId,
        CancellationToken cancellationToken = default);
}
