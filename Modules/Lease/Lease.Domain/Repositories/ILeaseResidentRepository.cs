using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface ILeaseResidentRepository
{
    Task<IReadOnlyList<LeaseResidentEntity>> GetByLeaseIdAsync(Guid leaseId, CancellationToken cancellationToken = default);
    Task<LeaseResidentEntity> AddAsync(LeaseResidentEntity resident, CancellationToken cancellationToken = default);
    Task UpdateAsync(LeaseResidentEntity resident, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid residentId, Guid deletedBy, DateTime deletedAtUtc, CancellationToken cancellationToken = default);
}
