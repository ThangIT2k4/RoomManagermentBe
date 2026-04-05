namespace Finance.Application.Services;

public sealed record LeaseSummary(Guid LeaseId, string Status, Guid? PrimaryResidentUserId);

public interface ILeaseReadGateway
{
    Task<LeaseSummary?> GetLeaseAsync(Guid leaseId, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>Lease IDs where the user is a resident (stub may return configured sample IDs).</summary>
    Task<IReadOnlyList<Guid>> GetLeaseIdsForResidentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
