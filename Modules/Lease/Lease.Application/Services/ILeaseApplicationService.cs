using Lease.Application.Dtos;

namespace Lease.Application.Services;

public interface ILeaseApplicationService
{
    Task<LeaseDto> CreateFromBookingAsync(Guid organizationId, Guid userId, CreateLeaseFromBookingRequest request, CancellationToken cancellationToken = default);
    Task<LeaseDto> CreateManualAsync(Guid organizationId, Guid userId, CreateManualLeaseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseDto>> SearchLeasesAsync(Guid organizationId, string? statuses, Guid? unitId, string? search, int page, int perPage, CancellationToken cancellationToken = default);
    Task<LeaseDto?> GetLeaseByIdAsync(Guid organizationId, Guid leaseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseDto>> GetTenantLeasesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<LeaseDto?> UpdateLeaseAsync(Guid organizationId, Guid userId, UpdateLeaseRequest request, CancellationToken cancellationToken = default);
    Task<LeaseDto> RenewLeaseAsync(Guid organizationId, Guid userId, RenewLeaseRequest request, CancellationToken cancellationToken = default);
    Task<LeaseDto?> TerminateLeaseAsync(Guid organizationId, Guid userId, TerminateLeaseRequest request, CancellationToken cancellationToken = default);

    Task<LeaseResidentDto> AddResidentAsync(Guid organizationId, Guid userId, AddResidentRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoveResidentAsync(Guid organizationId, Guid userId, Guid leaseId, Guid residentId, CancellationToken cancellationToken = default);
    Task<bool> SetPrimaryResidentAsync(Guid organizationId, Guid userId, Guid leaseId, Guid residentId, CancellationToken cancellationToken = default);
    Task<LeaseResidentDto?> LinkResidentUserAsync(Guid organizationId, Guid userId, LinkResidentUserRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseResidentDto>> GetResidentsAsync(Guid organizationId, Guid leaseId, CancellationToken cancellationToken = default);

    Task<bool> ApplyServiceSetAsync(Guid organizationId, Guid userId, ApplyServiceSetRequest request, CancellationToken cancellationToken = default);
    Task<LeaseServiceSetDto> UpsertServiceSetAsync(Guid organizationId, Guid userId, UpsertLeaseServiceSetRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaseServiceSetDto>> GetServiceSetsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<LeaseServiceSetDto?> GetServiceSetByIdAsync(Guid organizationId, Guid serviceSetId, CancellationToken cancellationToken = default);

    Task<PaymentCycleDto> UpsertPaymentCycleAsync(Guid organizationId, Guid userId, UpsertPaymentCycleRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentCycleDto>> GetPaymentCyclesAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> DeletePaymentCycleAsync(Guid organizationId, Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<MasterLeaseDto> UpsertMasterLeaseAsync(Guid organizationId, Guid userId, UpsertMasterLeaseRequest request, CancellationToken cancellationToken = default);
    Task<MasterLeaseDto?> TerminateMasterLeaseAsync(Guid organizationId, Guid userId, Guid id, DateOnly terminationDate, string reason, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MasterLeaseDto>> GetMasterLeasesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<int> RunExpiringLeaseCheckAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);
    Task<int> RunLeaseExpirySweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);
}
