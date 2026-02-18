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

public sealed class UserPermissionRepository(DataAccessAdapter adapter, ICacheService cacheService) : IUserPermissionRepository
{
    private const string CachePrefix = "identity:v1:user-permission";

    public async Task<Identity.Domain.Entities.UserPermissionEntity?> GetAsync(Guid userId, Guid permissionId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:permission-id:{permissionId}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserPermissionEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserPermission.Where(up => up.UserId == userId && up.PermissionId == permissionId).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserPermissionEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.UserPermissionEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.UserPermission.Where(up => up.UserId == userId).ToListAsync(ct);
        var entities = data.Select(up => up.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserPermissionEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.UserPermissionEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.UserPermission.Where(UserPermissionFields.UserId == userId);

        var predicate = QueryFilterBuilder.Build<UserPermissionFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserPermissionFields.PermissionId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserPermissionEntity>().ToList();
        var entities = list.Select(up => up.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.UserPermissionEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:user-id:{userId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<Identity.Domain.Entities.UserPermissionEntity> AddAsync(Identity.Domain.Entities.UserPermissionEntity userPermission, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserPermissionEntity(userPermission.PermissionId, userPermission.UserId)
        {
            IsGranted = userPermission.IsGranted
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return userPermission;
    }

    public async Task<Identity.Domain.Entities.UserPermissionEntity> UpdateAsync(Identity.Domain.Entities.UserPermissionEntity userPermission, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserPermission
            .Where(up => up.UserId == userPermission.UserId && up.PermissionId == userPermission.PermissionId)
            .FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.UserPermissionEntity;
        if (dal is null) return userPermission;

        dal.IsGranted = userPermission.IsGranted;

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return userPermission;
    }

    public async Task<bool> RemoveAsync(Identity.Domain.Entities.UserPermissionEntity userPermission, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserPermissionEntity(userPermission.PermissionId, userPermission.UserId);
        var removed = await adapter.DeleteEntityAsync(dal, ct);
        if (removed)
            await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
