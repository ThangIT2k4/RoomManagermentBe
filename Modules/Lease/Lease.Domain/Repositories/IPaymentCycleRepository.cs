using Lease.Domain.Entities;

namespace Lease.Domain.Repositories;

public interface IPaymentCycleRepository
{
    Task<PaymentCycleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentCycleEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<PaymentCycleEntity> AddAsync(PaymentCycleEntity cycle, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentCycleEntity cycle, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, Guid deletedBy, DateTime deletedAtUtc, CancellationToken cancellationToken = default);
}
