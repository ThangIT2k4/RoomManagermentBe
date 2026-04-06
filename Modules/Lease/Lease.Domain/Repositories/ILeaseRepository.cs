using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface ILeaseRepository
{
    Task<LeaseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeaseEntity> AddAsync(LeaseEntity lease, CancellationToken cancellationToken = default);
}

