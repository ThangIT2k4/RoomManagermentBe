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

public sealed class UserRoleRepository(DataAccessAdapter adapter, ICacheService cacheService) : IUserRoleRepository
{
    private const string CachePrefix = "identity:v1:user-role";

    public async Task<Identity.Domain.Entities.UserRoleEntity?> GetAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:role-id:{roleId}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserRoleEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserRole.Where(ur => ur.UserId == userId && ur.RoleId == roleId).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserRoleEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.UserRoleEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.UserRole.Where(ur => ur.UserId == userId).ToListAsync(ct);
        var entities = data.Select(ur => ur.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserRoleEntity>> GetByRoleIdAsync(Guid roleId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:role-id:{roleId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.UserRoleEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.UserRole.Where(ur => ur.RoleId == roleId).ToListAsync(ct);
        var entities = data.Select(ur => ur.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserRoleEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.UserRoleEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.UserRole.Where(UserRoleFields.UserId == userId);

        var predicate = QueryFilterBuilder.Build<UserRoleFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserRoleFields.RoleId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserRoleEntity>().ToList();
        var entities = list.Select(ur => ur.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.UserRoleEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserRoleEntity>> GetByRoleIdPagedAsync(Guid roleId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:role-id:{roleId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.UserRoleEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.UserRole.Where(UserRoleFields.RoleId == roleId);

        var predicate = QueryFilterBuilder.Build<UserRoleFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserRoleFields.UserId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserRoleEntity>().ToList();
        var entities = list.Select(ur => ur.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.UserRoleEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:role-id:{roleId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<Identity.Domain.Entities.UserRoleEntity> AddAsync(Identity.Domain.Entities.UserRoleEntity userRole, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserRoleEntity(userRole.RoleId, userRole.UserId);
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return userRole;
    }

    public async Task<bool> RemoveAsync(Identity.Domain.Entities.UserRoleEntity userRole, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserRoleEntity(userRole.RoleId, userRole.UserId);
        var removed = await adapter.DeleteEntityAsync(dal, ct);
        if (removed)
            await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
