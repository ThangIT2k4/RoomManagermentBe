using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface ICommissionEventRepository
{
    Task<CommissionEventEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CommissionEventEntity> AddAsync(CommissionEventEntity entity, CancellationToken cancellationToken = default);
    Task<CommissionEventEntity> UpdateAsync(CommissionEventEntity entity, CancellationToken cancellationToken = default);
}
