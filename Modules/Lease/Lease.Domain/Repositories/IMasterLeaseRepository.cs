using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface IMasterLeaseRepository
{
    Task<MasterLeaseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MasterLeaseEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<MasterLeaseEntity> AddAsync(MasterLeaseEntity masterLease, CancellationToken cancellationToken = default);
    Task UpdateAsync(MasterLeaseEntity masterLease, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, Guid deletedBy, DateTime deletedAtUtc, CancellationToken cancellationToken = default);
}
