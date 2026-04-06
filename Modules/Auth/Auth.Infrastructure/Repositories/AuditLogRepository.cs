using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Mapper;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using DalAuditLogEntity = RoomManagerment.Auth.EntityClasses.AuditLogEntity;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class AuditLogRepository(DataAccessAdapter adapter) : IAuditLogRepository
{
    public async Task<AuditLogEntity> AddAsync(AuditLogEntity auditLog, CancellationToken cancellationToken = default)
    {
        var dal = auditLog.ToPersistence();
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return auditLog;
    }

    public async Task<PagedResult<AuditLogEntity>> GetPagedAsync(
        Guid? actorId,
        Guid? organizationId,
        string? entityType,
        Guid? entityId,
        DateTime? fromUtc,
        DateTime? toUtc,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = PagingInput.Create(pageNumber, pageSize);
        var linq = new LinqMetaData(adapter);
        IQueryable<DalAuditLogEntity> query = linq.AuditLog;

        if (actorId.HasValue)
        {
            var id = actorId.Value;
            query = query.Where(x => x.ActorId == id);
        }

        if (organizationId.HasValue)
        {
            var orgId = organizationId.Value;
            query = query.Where(x => x.OrganizationId == orgId);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var et = entityType.Trim();
            query = query.Where(x => x.EntityType == et);
        }

        if (entityId.HasValue)
        {
            var eid = entityId.Value;
            query = query.Where(x => x.EntityId == eid);
        }

        if (fromUtc.HasValue)
        {
            var from = fromUtc.Value;
            query = query.Where(x => x.CreatedAt >= from);
        }

        if (toUtc.HasValue)
        {
            var to = toUtc.Value;
            query = query.Where(x => x.CreatedAt <= to);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(paging.Skip)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<AuditLogEntity>.Create(
            items.Select(x => x.ToDomain()).ToList(),
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }
}
