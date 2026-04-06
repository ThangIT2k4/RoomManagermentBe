using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Mapper;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
namespace Auth.Infrastructure.Repositories;

public sealed class UserProfileRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher) : IUserProfileRepository
{
    public async Task<UserProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserProfile.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<UserProfileEntity> AddAsync(UserProfileEntity profile, CancellationToken cancellationToken = default)
    {
        var dal = profile.ToPersistence();
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(profile, cancellationToken);
        return profile;
    }

    public async Task<UserProfileEntity> UpdateAsync(UserProfileEntity profile, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var existing = await linq.UserProfile.Where(x => x.UserId == profile.Id).FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException("User profile not found for update.");
        }

        var dal = profile.ToPersistence();
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(profile, cancellationToken);
        return profile;
    }

    public async Task<PagedResult<UserProfileEntity>> SearchPagedAsync(
        Guid? organizationId,
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = PagingInput.Create(pageNumber, pageSize);

        // Auth DB has no organization on users; avoid returning global rows when a tenant filter was requested.
        if (organizationId.HasValue)
        {
            return PagedResult<UserProfileEntity>.Create(
                Array.Empty<UserProfileEntity>(),
                paging.PageNumber,
                paging.PageSize,
                0);
        }

        var linq = new LinqMetaData(adapter);
        var normalizedSearchTerm = SearchInput.Normalize(searchTerm);

        IQueryable<RoomManagerment.Auth.EntityClasses.UserProfileEntity> query;

        if (normalizedSearchTerm is null)
        {
            query =
                from p in linq.UserProfile
                join u in linq.User on p.UserId equals u.Id
                where u.DeletedAt == null
                select p;
        }
        else
        {
            var term = normalizedSearchTerm;
            query =
                from p in linq.UserProfile
                join u in linq.User on p.UserId equals u.Id
                where u.DeletedAt == null &&
                      ((p.FullName != null && p.FullName.Contains(term)) ||
                       (p.IdNumber != null && p.IdNumber.Contains(term)) ||
                       (p.TaxCode != null && p.TaxCode.Contains(term)) ||
                       (p.Address != null && p.Address.Contains(term)) ||
                       (p.Note != null && p.Note.Contains(term)) ||
                       (p.AccountHolderName != null && p.AccountHolderName.Contains(term)) ||
                       (p.AccountNumber != null && p.AccountNumber.Contains(term)) ||
                       (p.SearchVector != null && p.SearchVector.Contains(term)) ||
                       (u.Email != null && u.Email.Contains(term)) ||
                       (u.Username != null && u.Username.Contains(term)) ||
                       (u.Phone != null && u.Phone.Contains(term)))
                select p;
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var pageRows = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(paging.Skip)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<UserProfileEntity>.Create(
            pageRows.Select(p => p.ToDomain()).ToList(),
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }

    private async Task PublishDomainEventsAsync(UserProfileEntity profile, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in profile.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        profile.ClearDomainEvents();
    }
}
