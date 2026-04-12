using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure;
using Auth.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using DalRoleEntity = RoomManagerment.Auth.EntityClasses.RoleEntity;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class RoleRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher,
    ILogger<RoleRepository> logger) : IRoleRepository
{
    private const string Repo = nameof(RoleRepository);

    public Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByIdAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var dal = await linq.Role.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<RoleEntity?> GetByKeyCodeAsync(RoleKey keyCode, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByKeyCodeAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var value = keyCode.Value;
            var dal = await linq.Role.Where(x => x.KeyCode == value).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<RoleEntity> AddAsync(RoleEntity role, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = role.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(role, cancellationToken);
            return role;
        });

    public Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var existing = await linq.Role.Where(x => x.Id == role.Id).FirstOrDefaultAsync(cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy vai trò để cập nhật.");
            }

            var dal = role.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(role, cancellationToken);
            return role;
        });

    public Task<PagedResult<RoleEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(SearchPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);
            var linq = new LinqMetaData(adapter);
            var normalizedSearchTerm = SearchInput.Normalize(searchTerm);

            IQueryable<DalRoleEntity> query = linq.Role;
            if (normalizedSearchTerm is not null)
            {
                query = query.Where(x =>
                    x.Name.Contains(normalizedSearchTerm) ||
                    (x.KeyCode != null && x.KeyCode.Contains(normalizedSearchTerm)) ||
                    (x.Description != null && x.Description.Contains(normalizedSearchTerm)));
            }

            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Name)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<RoleEntity>(
                items.Select(x => x.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    public Task<bool> ExistsByKeyCodeAsync(
        RoleKey keyCode,
        Guid? excludeRoleId = null,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(ExistsByKeyCodeAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var value = keyCode.Value;
            var query = linq.Role.Where(x => x.KeyCode == value);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(x => x.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        });

    private async Task PublishDomainEventsAsync(RoleEntity role, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in role.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        role.ClearDomainEvents();
    }
}
