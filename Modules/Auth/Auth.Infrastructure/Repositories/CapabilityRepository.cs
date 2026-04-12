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
using DalCapabilityEntity = RoomManagerment.Auth.EntityClasses.CapabilityEntity;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class CapabilityRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher,
    ILogger<CapabilityRepository> logger) : ICapabilityRepository
{
    private const string Repo = nameof(CapabilityRepository);

    public Task<CapabilityEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByIdAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var dal = await linq.Capability.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<CapabilityEntity?> GetByKeyCodeAsync(CapabilityKey keyCode, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByKeyCodeAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var value = keyCode.Value;
            var dal = await linq.Capability.Where(x => x.KeyCode == value).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<CapabilityEntity> AddAsync(CapabilityEntity capability, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = capability.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(capability, cancellationToken);
            return capability;
        });

    public Task<CapabilityEntity> UpdateAsync(CapabilityEntity capability, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var existing = await linq.Capability.Where(x => x.Id == capability.Id).FirstOrDefaultAsync(cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy quyền năng để cập nhật.");
            }

            var dal = capability.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(capability, cancellationToken);
            return capability;
        });

    public Task<PagedResult<CapabilityEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(SearchPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);
            var linq = new LinqMetaData(adapter);
            var normalizedSearchTerm = SearchInput.Normalize(searchTerm);

            IQueryable<DalCapabilityEntity> query = linq.Capability;
            if (normalizedSearchTerm is not null)
            {
                query = query.Where(x =>
                    x.Name.Contains(normalizedSearchTerm) ||
                    (x.KeyCode != null && x.KeyCode.Contains(normalizedSearchTerm)) ||
                    (x.Description != null && x.Description.Contains(normalizedSearchTerm)) ||
                    (x.Category != null && x.Category.Contains(normalizedSearchTerm)));
            }

            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CapabilityEntity>(
                items.Select(x => x.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    public Task<bool> ExistsByKeyCodeAsync(
        CapabilityKey keyCode,
        Guid? excludeCapabilityId = null,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(ExistsByKeyCodeAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var value = keyCode.Value;
            var query = linq.Capability.Where(x => x.KeyCode == value);

            if (excludeCapabilityId.HasValue)
            {
                query = query.Where(x => x.Id != excludeCapabilityId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        });

    private async Task PublishDomainEventsAsync(CapabilityEntity capability, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in capability.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        capability.ClearDomainEvents();
    }
}
