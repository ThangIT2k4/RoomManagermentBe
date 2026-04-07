using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface ILeaseServiceSetRepository
{
    Task<LeaseServiceSetEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseServiceSetEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<LeaseServiceSetEntity> AddAsync(LeaseServiceSetEntity serviceSet, CancellationToken cancellationToken = default);
    Task UpdateAsync(LeaseServiceSetEntity serviceSet, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, Guid deletedBy, DateTime deletedAtUtc, CancellationToken cancellationToken = default);
}
