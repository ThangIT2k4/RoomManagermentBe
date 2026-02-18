using Identity.Application.Services;
using Identity.Domain.Common;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Mapper;
using RoomManagerment.Identity.DatabaseSpecific;
using RoomManagerment.Identity.EntityClasses;
using RoomManagerment.Identity.FactoryClasses;
using RoomManagerment.Identity.HelperClasses;
using RoomManagerment.Identity.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace Identity.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(DataAccessAdapter adapter, ICacheService cacheService) : IRefreshTokenRepository
{
    private const string CachePrefix = "identity:v1:refresh-token";

    public async Task<Identity.Domain.Entities.RefreshTokenEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:id:{id}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.RefreshTokenEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.RefreshToken.Where(rt => rt.Id == id).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(15));

        return entity;
    }

    public async Task<Identity.Domain.Entities.RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:token:{token}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.RefreshTokenEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.RefreshToken.Where(rt => rt.Token == token).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(15));

        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RefreshTokenEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.RefreshTokenEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.RefreshToken.Where(rt => rt.UserId == userId).ToListAsync(ct);
        var entities = data.Select(rt => rt.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<PagedResult<Identity.Domain.Entities.RefreshTokenEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.RefreshTokenEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.RefreshToken.Where(RefreshTokenFields.UserId == userId);

        var predicate = QueryFilterBuilder.Build<RefreshTokenFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(RefreshTokenFields.CreatedAt.Descending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RefreshTokenEntity>().ToList();
        var entities = list.Select(rt => rt.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.RefreshTokenEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RefreshTokenEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.RefreshToken
            .Where(rt => rt.CreatedAt >= from && rt.CreatedAt <= to)
            .ToListAsync(ct);
        return data.Select(rt => rt.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.RefreshTokenEntity> AddAsync(Identity.Domain.Entities.RefreshTokenEntity refreshToken, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.RefreshTokenEntity
        {
            Id = refreshToken.Id,
            UserId = refreshToken.UserId,
            Token = refreshToken.Token.Value,
            ExpiresAt = refreshToken.ExpiresAt,
            IsRevoked = refreshToken.IsRevoked,
            CreatedAt = refreshToken.CreatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return refreshToken;
    }

    public async Task<Identity.Domain.Entities.RefreshTokenEntity> UpdateAsync(Identity.Domain.Entities.RefreshTokenEntity refreshToken, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.RefreshToken.Where(rt => rt.Id == refreshToken.Id).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.RefreshTokenEntity;
        if (dal is null) return refreshToken;

        dal.Token = refreshToken.Token.Value;
        dal.ExpiresAt = refreshToken.ExpiresAt;
        dal.IsRevoked = refreshToken.IsRevoked;

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return refreshToken;
    }

    public async Task<bool> RemoveAsync(Identity.Domain.Entities.RefreshTokenEntity refreshToken, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.RefreshTokenEntity(refreshToken.Id);
        var removed = await adapter.DeleteEntityAsync(dal, ct);
        if (removed)
            await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
