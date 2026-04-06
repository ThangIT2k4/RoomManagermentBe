using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface ICommissionPolicyRepository
{
    Task<CommissionPolicyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CommissionPolicyEntity> AddAsync(CommissionPolicyEntity entity, CancellationToken cancellationToken = default);
    Task<CommissionPolicyEntity> UpdateAsync(CommissionPolicyEntity entity, CancellationToken cancellationToken = default);
}
