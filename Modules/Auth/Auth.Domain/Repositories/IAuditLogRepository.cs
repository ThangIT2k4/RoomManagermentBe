using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLogEntity> AddAsync(AuditLogEntity auditLog, CancellationToken cancellationToken = default);
}

