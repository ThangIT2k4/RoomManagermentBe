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

public sealed class MenuRepository(DataAccessAdapter adapter, ICacheService cacheService) : IMenuRepository
{
    private const string CachePrefix = "identity:v1:menu";

    public async Task<Identity.Domain.Entities.MenuEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{id}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.MenuEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Menu.Where(m => m.Id == id).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<Identity.Domain.Entities.MenuEntity?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:code:{code}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.MenuEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.Menu.Where(m => m.Code == code).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();
        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.MenuEntity>> GetByParentIdAsync(Guid? parentId, QueryFilter? filter = null, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        IQueryable<RoomManagerment.Identity.EntityClasses.MenuEntity> query = linq.Menu;
        if (parentId.HasValue)
            query = query.Where(m => m.ParentId == parentId);
        else
            query = query.Where(m => m.ParentId == null);

        var data = await query.ToListAsync(ct);
        return data.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.MenuEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Menu.ToListAsync(ct);
        return data.Select(m => m.ToDomain()).ToList();
    }

    public async Task<PagedResult<Identity.Domain.Entities.MenuEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.Menu;

        var predicate = QueryFilterBuilder.Build<MenuFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(MenuFields.OrderIndex.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.MenuEntity>().ToList();
        var entities = list.Select(m => m.ToDomain()).ToList();
        return new PagedResult<Identity.Domain.Entities.MenuEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.MenuEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.Menu.Where(m => m.CreatedAt >= from && m.CreatedAt <= to).ToListAsync(ct);
        return data.Select(m => m.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.MenuEntity> AddAsync(Identity.Domain.Entities.MenuEntity menu, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.MenuEntity
        {
            Id = menu.Id,
            Code = menu.Code.Value,
            Label = menu.Label.Value,
            Path = menu.Path?.Value,
            Icon = menu.Icon,
            OrderIndex = menu.OrderIndex,
            ParentId = menu.ParentId,
            IsActive = menu.IsActive
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return menu;
    }

    public async Task<Identity.Domain.Entities.MenuEntity> UpdateAsync(Identity.Domain.Entities.MenuEntity menu, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Menu.Where(m => m.Id == menu.Id).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.MenuEntity;
        if (dal is null) return menu;
        dal.Code = menu.Code.Value;
        dal.Label = menu.Label.Value;
        dal.Path = menu.Path?.Value;
        dal.Icon = menu.Icon;
        dal.OrderIndex = menu.OrderIndex;
        dal.ParentId = menu.ParentId;
        dal.IsActive = menu.IsActive;
        dal.UpdatedAt = menu.UpdatedAt;
        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return menu;
    }
}
