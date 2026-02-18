using Identity.Application.Services;
using Identity.Domain.Common;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Mapper;
using RoomManagerment.Identity.DatabaseSpecific;
using RoomManagerment.Identity.FactoryClasses;
using RoomManagerment.Identity.HelperClasses;
using RoomManagerment.Identity.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace Identity.Infrastructure.Repositories;

public sealed class PermissionRepository(DataAccessAdapter adapter, ICacheService cacheService) : IPermissionRepository
{
    private const string CachePrefix = "identity:v1:permission";

    public async Task<Identity.Domain.Entities.PermissionEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{id}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.PermissionEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Permission.Where(p => p.Id == id).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<Identity.Domain.Entities.PermissionEntity?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:code:{code}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.PermissionEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Permission.Where(p => p.Code == code).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.PermissionEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Permission.ToListAsync(ct);
        return data.Select(p => p.ToDomain()).ToList();
    }

    public async Task<PagedResult<Identity.Domain.Entities.PermissionEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.Permission;

        var predicate = QueryFilterBuilder.Build<PermissionFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(PermissionFields.Id.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.PermissionEntity>().ToList();
        var entities = list.Select(p => p.ToDomain()).ToList();
        return new PagedResult<Identity.Domain.Entities.PermissionEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.PermissionEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Permission.Where(p => p.CreatedAt >= from && p.CreatedAt <= to).ToListAsync(ct);
        return data.Select(p => p.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.PermissionEntity> AddAsync(Identity.Domain.Entities.PermissionEntity permission, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.PermissionEntity
        {
            Id = permission.Id,
            Code = permission.Code.Value,
            Name = permission.Name
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return permission;
    }

    public async Task<Identity.Domain.Entities.PermissionEntity> UpdateAsync(Identity.Domain.Entities.PermissionEntity permission, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Permission.Where(p => p.Id == permission.Id).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.PermissionEntity;
        if (dal is null) return permission;
        dal.Code = permission.Code.Value;
        dal.Name = permission.Name;
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return permission;
    }
}
