using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure;
using Auth.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class UserRepository(
    DataAccessAdapter adapter,
    ICacheService cacheService,
    IIntegrationEventPublisher eventPublisher,
    ILogger<UserRepository> logger) : IUserRepository
{
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);
    private const string Repo = nameof(UserRepository);

    public Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByIdAsync), cancellationToken, async () =>
        {
            var cacheKey = BuildUserIdCacheKey(id);
            var cached = await cacheService.GetAsync<UserEntity>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var linq = new LinqMetaData(adapter);
            var dal = await linq.User.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            var domain = dal?.ToDomain();
            if (domain is not null)
            {
                await TryCacheAsync(domain, cancellationToken);
            }

            return domain;
        });

    public Task<UserEntity?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByEmailAsync), cancellationToken, async () =>
        {
            var cacheKey = BuildUserEmailCacheKey(email.Value);
            var cached = await cacheService.GetAsync<UserEntity>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var linq = new LinqMetaData(adapter);
            var dal = await linq.User.Where(x => x.Email == email.Value).FirstOrDefaultAsync(cancellationToken);
            var domain = dal?.ToDomain();
            if (domain is not null)
            {
                await TryCacheAsync(domain, cancellationToken);
            }

            return domain;
        });

    public Task<UserEntity?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByUsernameAsync), cancellationToken, async () =>
        {
            var cacheKey = BuildUserUsernameCacheKey(username.Value);
            var cached = await cacheService.GetAsync<UserEntity>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var linq = new LinqMetaData(adapter);
            var dal = await linq.User.Where(x => x.Username == username.Value).FirstOrDefaultAsync(cancellationToken);
            var domain = dal?.ToDomain();
            if (domain is not null)
            {
                await TryCacheAsync(domain, cancellationToken);
            }

            return domain;
        });

    public Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = user.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(user, cancellationToken);
            await TryCacheAsync(user, cancellationToken);
            return user;
        });

    public Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            var existing = await GetDbByIdAsync(user.Id, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException($"Không tìm thấy người dùng {user.Id} để cập nhật.");
            }

            var previousDomain = existing.ToDomain();
            existing.ApplyFromDomain(user);
            await adapter.SaveEntityAsync(existing, true, false, cancellationToken);
            await PublishDomainEventsAsync(user, cancellationToken);
            await TryInvalidateAsync(previousDomain, cancellationToken);
            await TryCacheAsync(user, cancellationToken);
            return user;
        });

    public Task<PagedResult<UserEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(SearchPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);

            var linq = new LinqMetaData(adapter);
            var normalizedSearchTerm = SearchInput.Normalize(searchTerm);
            var query = ApplyUserSearch(linq.User, normalizedSearchTerm, includeDeleted);

            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<UserEntity>(
                items.Select(x => x.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    public Task<long> CountAsync(
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(CountAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var normalizedSearchTerm = SearchInput.Normalize(searchTerm);
            var query = ApplyUserSearch(linq.User, normalizedSearchTerm, includeDeleted);
            return await query.LongCountAsync(cancellationToken);
        });

    public Task<bool> ExistsByEmailAsync(
        Email email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(ExistsByEmailAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var query = linq.User.Where(x => x.Email == email.Value && x.DeletedAt == null);

            if (excludeUserId.HasValue)
            {
                query = query.Where(x => x.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        });

    public Task<bool> ExistsByUsernameAsync(
        Username username,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(ExistsByUsernameAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var query = linq.User.Where(x => x.Username == username.Value && x.DeletedAt == null);

            if (excludeUserId.HasValue)
            {
                query = query.Where(x => x.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        });

    public Task SoftDeleteAsync(
        Guid userId,
        Guid deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(SoftDeleteAsync), cancellationToken, async () =>
        {
            if (deletedAt.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("deletedAt phải ở định dạng UTC.", nameof(deletedAt));
            }

            var dal = await GetDbByIdAsync(userId, cancellationToken);

            if (dal is null)
            {
                return;
            }

            dal.DeletedAt = deletedAt;
            dal.DeletedBy = deletedBy;
            dal.UpdatedAt = deletedAt;

            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await TryInvalidateAsync(dal.ToDomain(), cancellationToken);
        });

    private async Task<RoomManagerment.Auth.EntityClasses.UserEntity?> GetDbByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.User.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task TryCacheAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await cacheService.SetAsync(BuildUserIdCacheKey(user.Id), user, CacheExpiry, cancellationToken);
        await cacheService.SetAsync(BuildUserEmailCacheKey(user.Email.Value), user, CacheExpiry, cancellationToken);

        if (user.Username is not null)
        {
            await cacheService.SetAsync(BuildUserUsernameCacheKey(user.Username.Value), user, CacheExpiry, cancellationToken);
        }
    }

    private async Task TryInvalidateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(BuildUserIdCacheKey(user.Id), cancellationToken);
        await cacheService.RemoveAsync(BuildUserEmailCacheKey(user.Email.Value), cancellationToken);

        if (user.Username is not null)
        {
            await cacheService.RemoveAsync(BuildUserUsernameCacheKey(user.Username.Value), cancellationToken);
        }
    }

    private async Task PublishDomainEventsAsync(UserEntity user, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in user.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        user.ClearDomainEvents();
    }

    private static string BuildUserIdCacheKey(Guid id) => $"auth:user:id:{id}";
    private static string BuildUserEmailCacheKey(string email) => $"auth:user:email:{email}";
    private static string BuildUserUsernameCacheKey(string username) => $"auth:user:username:{username}";

    private static IQueryable<RoomManagerment.Auth.EntityClasses.UserEntity> ApplyUserSearch(
        IQueryable<RoomManagerment.Auth.EntityClasses.UserEntity> query,
        string? searchTerm,
        bool includeDeleted)
    {
        if (!includeDeleted)
        {
            query = query.Where(x => x.DeletedAt == null);
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        return query.Where(x =>
            x.Email.Contains(searchTerm) ||
            (x.Username != null && x.Username.Contains(searchTerm)) ||
            (x.Phone != null && x.Phone.Contains(searchTerm)) ||
            (x.GoogleId != null && x.GoogleId.Contains(searchTerm)));
    }
}
