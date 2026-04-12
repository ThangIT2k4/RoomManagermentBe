using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure;
using Auth.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class UserProfileRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher,
    ILogger<UserProfileRepository> logger) : IUserProfileRepository
{
    private const string Repo = nameof(UserProfileRepository);

    public Task<UserProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByUserIdAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var dal = await linq.UserProfile.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<UserProfileEntity> AddAsync(UserProfileEntity profile, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = profile.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(profile, cancellationToken);
            return profile;
        });

    public Task<UserProfileEntity> UpdateAsync(UserProfileEntity profile, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var existing = await linq.UserProfile.Where(x => x.UserId == profile.Id).FirstOrDefaultAsync(cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy hồ sơ người dùng để cập nhật.");
            }

            var dal = profile.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(profile, cancellationToken);
            return profile;
        });

    public Task<PagedResult<UserProfileEntity>> SearchPagedAsync(
        Guid? organizationId,
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(SearchPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);

            if (organizationId.HasValue)
            {
                return new PagedResult<UserProfileEntity>(
                    Array.Empty<UserProfileEntity>(),
                    0,
                    paging.PageNumber,
                    paging.PageSize);
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

            return new PagedResult<UserProfileEntity>(
                pageRows.Select(p => p.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    private async Task PublishDomainEventsAsync(UserProfileEntity profile, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in profile.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        profile.ClearDomainEvents();
    }
}
