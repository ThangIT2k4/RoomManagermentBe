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

public sealed class UserProfileRepository(DataAccessAdapter adapter, ICacheService cacheService) : IUserProfileRepository
{
    private const string CachePrefix = "identity:v1:user-profile";

    public async Task<Identity.Domain.Entities.UserProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:user-id:{userId}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserProfileEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserProfile.Where(p => p.UserId == userId).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserProfileEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:page:{page}:page-size:{pageSize}";
            var cached = await cacheService.GetAsync<PagedResult<Identity.Domain.Entities.UserProfileEntity>>(cacheKey);
            if (cached is not null) return cached;
        }

        var qf = new QueryFactory();
        var baseQuery = qf.UserProfile;

        var predicate = QueryFilterBuilder.Build<UserProfileFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserProfileFields.UserId.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserProfileEntity>().ToList();
        var entities = list.Select(p => p.ToDomain()).ToList();
        var result = new PagedResult<Identity.Domain.Entities.UserProfileEntity>(entities, totalCount, page, pageSize);

        if (filter is null || filter.Conditions.Count == 0)
        {
            var cacheKey = $"{CachePrefix}:page:{page}:page-size:{pageSize}";
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserProfileEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.UserProfile
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .ToListAsync(ct);
        return data.Select(p => p.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.UserProfileEntity> AddAsync(Identity.Domain.Entities.UserProfileEntity profile, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserProfileEntity
        {
            UserId = profile.UserId,
            FullName = profile.FullName?.Value ?? string.Empty,
            Phone = profile.Phone is not null ? profile.Phone.Value : null,
            AvatarUrl = profile.AvatarUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return profile;
    }

    public async Task<Identity.Domain.Entities.UserProfileEntity> UpdateAsync(Identity.Domain.Entities.UserProfileEntity profile, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserProfile.Where(p => p.UserId == profile.UserId).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.UserProfileEntity;
        if (dal is null) return profile;

        dal.FullName = profile.FullName?.Value ?? string.Empty;
        dal.Phone = profile.Phone is not null ? profile.Phone.Value : null;
        dal.AvatarUrl = profile.AvatarUrl;
        dal.UpdatedAt = profile.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, ct);
        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return profile;
    }
}
