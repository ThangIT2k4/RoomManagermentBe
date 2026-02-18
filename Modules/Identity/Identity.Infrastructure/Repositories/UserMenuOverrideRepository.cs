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

public sealed class UserMenuOverrideRepository(DataAccessAdapter adapter, ICacheService cacheService) : IUserMenuOverrideRepository
{
    private const string CachePrefix = "identity:v1:user-menu-override";

    public async Task<Identity.Domain.Entities.UserMenuOverrideEntity?> GetAsync(Guid userId, Guid menuId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:menu-id:{menuId}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserMenuOverrideEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserMenuOverride.Where(umo => umo.UserId == userId && umo.MenuId == menuId).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserMenuOverrideEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.UserMenuOverrideEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.UserMenuOverride.Where(umo => umo.UserId == userId).ToListAsync(ct);
        var entities = data.Select(umo => umo.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserMenuOverrideEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.UserMenuOverrideEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.UserMenuOverride.Where(UserMenuOverrideFields.UserId == userId);

        var predicate = QueryFilterBuilder.Build<UserMenuOverrideFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserMenuOverrideFields.MenuId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserMenuOverrideEntity>().ToList();
        var entities = list.Select(umo => umo.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.UserMenuOverrideEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<Identity.Domain.Entities.UserMenuOverrideEntity> AddAsync(Identity.Domain.Entities.UserMenuOverrideEntity userMenuOverride, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserMenuOverrideEntity(userMenuOverride.MenuId, userMenuOverride.UserId)
        {
            IsVisible = userMenuOverride.IsVisible
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return userMenuOverride;
    }

    public async Task<Identity.Domain.Entities.UserMenuOverrideEntity> UpdateAsync(Identity.Domain.Entities.UserMenuOverrideEntity userMenuOverride, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserMenuOverride
            .Where(umo => umo.UserId == userMenuOverride.UserId && umo.MenuId == userMenuOverride.MenuId)
            .FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.UserMenuOverrideEntity;
        if (dal is null) return userMenuOverride;

        dal.IsVisible = userMenuOverride.IsVisible;

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return userMenuOverride;
    }

    public async Task<bool> RemoveAsync(Identity.Domain.Entities.UserMenuOverrideEntity userMenuOverride, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserMenuOverrideEntity(userMenuOverride.MenuId, userMenuOverride.UserId);
        var removed = await adapter.DeleteEntityAsync(dal, ct);
        if (removed)
            await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
