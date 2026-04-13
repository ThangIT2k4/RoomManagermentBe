using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure;
using Auth.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class SessionRepository(
    DataAccessAdapter adapter,
    ICacheService cacheService,
    IIntegrationEventPublisher eventPublisher,
    ILogger<SessionRepository> logger) : ISessionRepository
{
    private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromHours(1);
    private const string Repo = nameof(SessionRepository);

    public Task<SessionEntity?> GetByIdAsync(string sessionId, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByIdAsync), cancellationToken, async () =>
        {
            var cacheKey = BuildCacheKey(sessionId);
            var cached = await cacheService.GetAsync<SessionEntity>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var dal = await QuerySessionByIdAsync(sessionId, cancellationToken);
            if (dal is null)
            {
                return null;
            }

            var session = dal.ToDomain();
            await TryCacheSessionAsync(session, cancellationToken);
            return session;
        });

    public Task<SessionEntity> AddAsync(SessionEntity session, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = session.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(session, cancellationToken);
            await TryCacheSessionAsync(session, cancellationToken);
            return session;
        });

    public Task<SessionEntity> UpdateAsync(SessionEntity session, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            if (session.IsRevoked)
            {
                await DeleteAsync(session.Id, cancellationToken);
                await PublishDomainEventsAsync(session, cancellationToken);
                return session;
            }

            var existing = await QuerySessionByIdAsync(session.Id, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy phiên đăng nhập để cập nhật.");
            }

            existing.ApplyFromDomain(session);
            await adapter.SaveEntityAsync(existing, true, false, cancellationToken);
            await PublishDomainEventsAsync(session, cancellationToken);
            await TryCacheSessionAsync(session, cancellationToken);
            return session;
        });

    public Task DeleteAsync(string sessionId, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(DeleteAsync), cancellationToken, async () =>
        {
            var dal = await QuerySessionByIdAsync(sessionId, cancellationToken);
            if (dal is null)
            {
                return;
            }

            await adapter.DeleteEntityAsync(dal, cancellationToken);
            await cacheService.RemoveAsync(BuildCacheKey(sessionId), cancellationToken);
        });

    public Task<PagedResult<SessionEntity>> GetByUserPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        bool includeExpired = false,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByUserPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);
            var linq = new LinqMetaData(adapter);
            var query = ApplyActiveFilter(linq.Session.Where(x => x.UserId == userId), includeExpired, DateTimeOffset.UtcNow);

            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.LastActivity)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<SessionEntity>(
                items.Select(x => x.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    public Task<IReadOnlyCollection<SessionEntity>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetActiveByUserAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var items = await ApplyActiveFilter(linq.Session.Where(x => x.UserId == userId), false, DateTimeOffset.UtcNow)
                .OrderByDescending(x => x.LastActivity)
                .ToListAsync(cancellationToken);

            return (IReadOnlyCollection<SessionEntity>)items.Select(x => x.ToDomain()).ToList();
        });

    public Task<long> DeleteExpiredAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(DeleteExpiredAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var expiredSessions = await linq.Session
                .Where(x => x.ExpiresAt != null && x.ExpiresAt <= now.UtcDateTime)
                .ToListAsync(cancellationToken);

            foreach (var session in expiredSessions)
            {
                await cacheService.RemoveAsync(BuildCacheKey(session.Id), cancellationToken);
                await adapter.DeleteEntityAsync(session, cancellationToken);
            }

            return (long)expiredSessions.Count;
        });

    public Task<long> DeleteAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(DeleteAllByUserAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var sessions = await linq.Session.Where(x => x.UserId == userId).ToListAsync(cancellationToken);

            foreach (var session in sessions)
            {
                await cacheService.RemoveAsync(BuildCacheKey(session.Id), cancellationToken);
                await adapter.DeleteEntityAsync(session, cancellationToken);
            }

            return (long)sessions.Count;
        });

    public Task<long> DeleteAllByUserExceptAsync(
        Guid userId,
        string exceptSessionId,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(DeleteAllByUserExceptAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var sessions = await linq.Session.Where(x => x.UserId == userId && x.Id != exceptSessionId).ToListAsync(cancellationToken);

            foreach (var session in sessions)
            {
                await cacheService.RemoveAsync(BuildCacheKey(session.Id), cancellationToken);
                await adapter.DeleteEntityAsync(session, cancellationToken);
            }

            return (long)sessions.Count;
        });

    private async Task<RoomManagerment.Auth.EntityClasses.SessionEntity?> QuerySessionByIdAsync(string sessionId, CancellationToken cancellationToken)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.Session.Where(x => x.Id == sessionId).FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<RoomManagerment.Auth.EntityClasses.SessionEntity> ApplyActiveFilter(
        IQueryable<RoomManagerment.Auth.EntityClasses.SessionEntity> query,
        bool includeExpired,
        DateTimeOffset now)
    {
        if (includeExpired)
        {
            return query;
        }

        return query.Where(x => x.ExpiresAt == null || x.ExpiresAt > now.UtcDateTime);
    }

    private async Task TryCacheSessionAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        var expiry = session.ExpiresAt.HasValue
            ? session.ExpiresAt.Value - DateTimeOffset.UtcNow
            : DefaultCacheExpiry;

        if (expiry <= TimeSpan.Zero)
        {
            return;
        }

        await cacheService.SetAsync(BuildCacheKey(session.Id), session, expiry, cancellationToken);
    }

    private async Task PublishDomainEventsAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in session.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        session.ClearDomainEvents();
    }

    private static string BuildCacheKey(string sessionId) => $"auth:session:{sessionId}";
}
