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

public sealed class UserRepository(DataAccessAdapter adapter, ICacheService cacheService) : IUserRepository
{
    private const string CachePrefix = "identity:v1:user";

    public async Task<Identity.Domain.Entities.UserEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:id:{id}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.User.Where(u => u.Id == id).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<Identity.Domain.Entities.UserEntity?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:username:{username}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.User.Where(u => u.Username == username).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<Identity.Domain.Entities.UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}:entity:email:{email}";
        var cached = await cacheService.GetAsync<Identity.Domain.Entities.UserEntity>(cacheKey);
        if (cached is not null) return cached;

        var linq = new LinqMetaData(adapter);
        var dal = await linq.User.Where(u => u.Email == email).FirstOrDefaultAsync(ct);
        var entity = dal?.ToDomain();

        if (entity is not null)
            await cacheService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));

        return entity;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.User.AnyAsync(u => u.Username == username, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.User.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<PagedResult<Identity.Domain.Entities.UserEntity>> GetPagedAsync(int page, int pageSize, QueryFilter? filter = null, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.User;

        var predicate = QueryFilterBuilder.Build<UserFields>(filter);
        if (predicate is not null)
            baseQuery = baseQuery.Where(predicate);

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery.OrderBy(UserFields.Id.Ascending()).Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Identity.EntityClasses.UserEntity>().ToList();
        var entities = list.Select(u => u.ToDomain()).ToList();
        return new PagedResult<Identity.Domain.Entities.UserEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Identity.Domain.Entities.UserEntity>> GetByCreatedAtRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var data = await linq.User
            .Where(u => u.CreatedAt >= from && u.CreatedAt <= to)
            .ToListAsync(ct);
        return data.Select(u => u.ToDomain()).ToList();
    }

    public async Task<Identity.Domain.Entities.UserEntity> AddAsync(Identity.Domain.Entities.UserEntity user, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Identity.EntityClasses.UserEntity
        {
            Id = user.Id,
            Username = user.Username.Value,
            Email = user.Email.Value,
            PasswordHash = user.PasswordHash.Value,
            Status = (short)user.Status,
            CreatedAt = user.CreatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, ct);

        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return user;
    }

    public async Task<Identity.Domain.Entities.UserEntity> UpdateAsync(Identity.Domain.Entities.UserEntity user, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.User.Where(u => u.Id == user.Id).FirstOrDefaultAsync(ct) as RoomManagerment.Identity.EntityClasses.UserEntity;
        if (dal is null) return user;

        dal.Username = user.Username.Value;
        dal.Email = user.Email.Value;
        dal.PasswordHash = user.PasswordHash.Value;
        dal.Status = (short)user.Status;
        dal.UpdatedAt = user.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, ct);

        await cacheService.RemoveByPatternAsync($"{CachePrefix}:*");

        return user;
    }
}
