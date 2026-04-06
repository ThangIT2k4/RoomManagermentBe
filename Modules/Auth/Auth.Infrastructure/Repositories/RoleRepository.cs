using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Mapper;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using DalRoleEntity = RoomManagerment.Auth.EntityClasses.RoleEntity;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class RoleRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher) : IRoleRepository
{
    public async Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Role.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<RoleEntity?> GetByKeyCodeAsync(RoleKey keyCode, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var value = keyCode.Value;
        var dal = await linq.Role.Where(x => x.KeyCode == value).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<RoleEntity> AddAsync(RoleEntity role, CancellationToken cancellationToken = default)
    {
        var dal = role.ToPersistence();
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(role, cancellationToken);
        return role;
    }

    public async Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var existing = await linq.Role.Where(x => x.Id == role.Id).FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException("Role not found for update.");
        }

        var dal = role.ToPersistence();
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(role, cancellationToken);
        return role;
    }

    public async Task<PagedResult<RoleEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
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

        return PagedResult<RoleEntity>.Create(
            items.Select(x => x.ToDomain()).ToList(),
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }

    public async Task<bool> ExistsByKeyCodeAsync(
        RoleKey keyCode,
        Guid? excludeRoleId = null,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var value = keyCode.Value;
        var query = linq.Role.Where(x => x.KeyCode == value);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(x => x.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(RoleEntity role, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in role.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        role.ClearDomainEvents();
    }
}
