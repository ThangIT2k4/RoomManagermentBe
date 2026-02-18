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

public sealed class RolePermissionRepository(DataAccessAdapter adapter, ICacheService cacheService) : IRolePermissionRepository
{
    private const string CachePrefix = "identity:v1:role-permission";

    public async Task<Identity.Domain.Entities.RolePermissionEntity?> GetAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:role-id:{roleId}:permission-id:{permissionId}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.RolePermissionEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.RolePermission.Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RolePermissionEntity>> GetByRoleIdAsync(Guid roleId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:role-id:{roleId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.RolePermissionEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.RolePermission.Where(rp => rp.RoleId == roleId).ToListAsync(ct);
        var entities = data.Select(rp => rp.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RolePermissionEntity>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:permission-id:{permissionId}:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Identity.Domain.Entities.RolePermissionEntity>>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var data = await linq.RolePermission.Where(rp => rp.PermissionId == permissionId).ToListAsync(ct);
        var entities = data.Select(rp => rp.ToDomain()).ToList();

        await cacheService.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5));
        return entities;
    }

    public async Task<PagedResult<Identity.Domain.Entities.RolePermissionEntity>> GetByRoleIdPagedAsync(Guid roleId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:role-id:{roleId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.RolePermissionEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.RolePermission.Where(RolePermissionFields.RoleId == roleId);

        var predicate = QueryFilterBuilder.Build<RolePermissionFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(RolePermissionFields.PermissionId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.RolePermissionEntity>().ToList();
        var entities = list.Select(rp => rp.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.RolePermissionEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:role-id:{roleId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<PagedResult<Identity.Domain.Entities.RolePermissionEntity>> GetByPermissionIdPagedAsync(Guid permissionId, int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:permission-id:{permissionId}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.RolePermissionEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.RolePermission.Where(RolePermissionFields.PermissionId == permissionId);

        var predicate = QueryFilterBuilder.Build<RolePermissionFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(RolePermissionFields.RoleId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.RolePermissionEntity>().ToList();
        var entities = list.Select(rp => rp.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.RolePermissionEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:permission-id:{permissionId}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<Identity.Domain.Entities.RolePermissionEntity> AddAsync(Identity.Domain.Entities.RolePermissionEntity rolePermission, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.RolePermissionEntity(rolePermission.PermissionId, rolePermission.RoleId);
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return rolePermission;
    }

    public async Task<bool> RemoveAsync(Identity.Domain.Entities.RolePermissionEntity rolePermission, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.RolePermissionEntity(rolePermission.PermissionId, rolePermission.RoleId);
        var removed = await adapter.DeleteEntityAsync(dal, ct);
        if (removed)
            await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
