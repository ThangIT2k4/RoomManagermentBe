using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface ILeaseRepository
{
    Task<LeaseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeaseEntity?> GetByBookingIdAsync(Guid bookingId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseEntity>> GetByUnitIdAsync(Guid unitId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseEntity>> SearchAsync(Guid organizationId, IReadOnlyCollection<string>? statuses, Guid? unitId, string? search, int page, int perPage, CancellationToken cancellationToken = default);
    Task<LeaseEntity> AddAsync(LeaseEntity lease, CancellationToken cancellationToken = default);
    Task UpdateAsync(LeaseEntity lease, CancellationToken cancellationToken = default);
}

