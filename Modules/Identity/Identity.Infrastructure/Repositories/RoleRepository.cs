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

public sealed class RoleRepository(DataAccessAdapter adapter, ICacheService cacheService) : IRoleRepository
{
    private const string CachePrefix = "identity:v1:role";

    public async Task<Identity.Domain.Entities.RoleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{id}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.RoleEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Role.Where(r => r.Id == id).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<Identity.Domain.Entities.RoleEntity?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:code:{code}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.RoleEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Role.Where(r => r.Code == code).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RoleEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Role.ToListAsync(ct);
        return data.Select(r => r.ToDomain()).ToList();
    }

    public async Task<PagedResult<Identity.Domain.Entities.RoleEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.Role;

        var predicate = QueryFilterBuilder.Build<RoleFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(RoleFields.Id.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.RoleEntity>().ToList();
        var entities = list.Select(r => r.ToDomain()).ToList();
        return new PagedResult<Identity.Domain.Entities.RoleEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.RoleEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Role.Where(r => r.CreatedAt >= from && r.CreatedAt <= to).ToListAsync(ct);
        return data.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.RoleEntity> AddAsync(Identity.Domain.Entities.RoleEntity role, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.RoleEntity
        {
            Id = role.Id,
            Code = role.Code.Value,
            Name = role.Name.Value
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return role;
    }

    public async Task<Identity.Domain.Entities.RoleEntity> UpdateAsync(Identity.Domain.Entities.RoleEntity role, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Role.Where(r => r.Id == role.Id).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.RoleEntity;
        if (dal is null) return role;
        dal.Code = role.Code.Value;
        dal.Name = role.Name.Value;
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return role;
    }
}
