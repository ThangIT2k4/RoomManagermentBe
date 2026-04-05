using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public interface IDepositRefundRepository
{
    Task AddAsync(DepositRefundEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(DepositRefundEntity entity, CancellationToken cancellationToken = default);

    Task<DepositRefundEntity?> GetByIdAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default);

    Task<bool> HasPendingOrApprovedForLeaseAsync(Guid leaseId, CancellationToken cancellationToken = default);
}
