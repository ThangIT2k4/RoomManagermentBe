using Identity.Application.Services;
using Identity.Domain.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Mapper;
using RoomManagerment.Identity.DatabaseSpecific;
using RoomManagerment.Identity.FactoryClasses;
using RoomManagerment.Identity.HelperClasses;
using RoomManagerment.Identity.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace Identity.Infrastructure.Repositories;

public sealed class MenuPermissionRepository(DataAccessAdapter adapter, ICacheService cacheService) : IMenuPermissionRepository
{
    private readonly DataAccessAdapter _adapter = adapter;
    private readonly ICacheService _cacheService = cacheService;
    private const string CachePrefix = "identity:v1:menu-permission";
    public async Task<MenuPermissionEntity?> GetAsync(Guid menuId, string permissionCode, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{menuId}:permission:{permissionCode}";
        var cacheValue = await _cacheService.GetAsync<MenuPermissionEntity>(cacheKey);

        if (cacheValue is not null)
        {
            return cacheValue;
        }

        var linq = new LinqMetaData(_adapter);
        var data = await linq.MenuPermission
            .Where(mp => mp.MenuId == menuId && mp.PermissionCode == permissionCode)
            .FirstOrDefaultAsync(ct);
            
        var entity = data?.ToDomain();

        await _cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
        return entity;
    }

    public async Task<IReadOnlyList<MenuPermissionEntity>> GetByMenuIdAsync(Guid menuId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{menuId}:all";
        var cacheValue = await _cacheService.GetAsync<IReadOnlyList<MenuPermissionEntity>>(cacheKey);
        if (cacheValue is not null)
        {
            return cacheValue;
        }

        var linq = new LinqMetaData(_adapter);
        var data = await linq.MenuPermission
            .Where(mp => mp.MenuId == menuId)
            .ToListAsync(ct);
            
        var entities = data.Select(mp => mp.ToDomain()).ToList();
        await _cacheService.SetAsync(cacheKey, entities, TimeSpan.FromHours(1));
        return entities;
    }

    public async Task<PagedResult<MenuPermissionEntity>> GetByMenuIdPagedAsync(Guid menuId, int page, int pageSize, QueryFilter? filter = null,
        CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:paged:menu-id:{menuId}:page:{page}:page-size:{pageSize}";
            var cacheValue = await _cacheService.GetAsync<PagedResult<MenuPermissionEntity>>(cacheKey);
            if (cacheValue is not null)
                return cacheValue;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.MenuPermission
            .Where(MenuPermissionFields.MenuId == menuId);

        var predicateExpression = QueryFilterBuilder.Build<MenuPermissionFields>(filter);
        if (predicateExpression is not null)
        {
            baseQuery = baseQuery.Where(predicateExpression);
        }

        var countQuery = baseQuery.Select(Functions.CountRow());
        var totalCount = await _adapter.FetchScalarAsync<int>(countQuery, ct);

        var query = baseQuery
            .OrderBy(MenuPermissionFields.MenuId.Ascending())
            .Page(page, pageSize);

        var data = await _adapter.FetchQueryAsync(query, ct);
        
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.MenuPermissionEntity>().ToList();
        var entities = list.Select(mp => mp.ToDomain()).ToList();
        var result = new PagedResult<MenuPermissionEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:paged:menu-id:{menuId}:page:{page}:page-size:{pageSize}";
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }
    

    public async Task<MenuPermissionEntity> AddAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default)
    {
        var dal = menuPermission.ToDal();
        await _adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await _cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return menuPermission;
    }

    public async Task<bool> RemoveAsync(MenuPermissionEntity menuPermission, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.MenuPermissionEntity(menuPermission.Id.MenuId, menuPermission.Id.PermissionCode.Value);
        var removed = await _adapter.DeleteEntityAsync(dal, cancellationToken);
        if (removed)
            await _cacheService.RemoveByPatternAsync($"{CachePrefix}:*");
        return removed;
    }
}
