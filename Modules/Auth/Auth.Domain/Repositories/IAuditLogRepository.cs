using Auth.Domain.Common;
using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLogEntity> AddAsync(AuditLogEntity auditLog, CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLogEntity>> GetPagedAsync(
        Guid? actorId,
        Guid? organizationId,
        string? entityType,
        Guid? entityId,
        DateTime? fromUtc,
        DateTime? toUtc,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
