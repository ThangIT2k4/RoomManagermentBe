namespace Finance.Domain.Entities;

public sealed class DepositRefundEntity
{
    public Guid Id { get; private set; }
    public Guid LeaseId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? TenantId { get; private set; }
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = "pending";
    public string? Notes { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? PaidBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private DepositRefundEntity()
    {
    }

    public static DepositRefundEntity CreatePending(
        Guid leaseId,
        Guid organizationId,
        Guid? tenantId,
        decimal amount,
        string? notes,
        Guid? createdBy)
    {
        if (leaseId == Guid.Empty || organizationId == Guid.Empty)
        {
            throw new ArgumentException("Lease and organization are required.");
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        var now = DateTime.UtcNow;
        return new DepositRefundEntity
        {
            Id = Guid.CreateVersion7(),
            LeaseId = leaseId,
            OrganizationId = organizationId,
            TenantId = tenantId,
            Amount = amount,
            Status = "pending",
            Notes = notes?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = now
        };
    }

    public static DepositRefundEntity FromPersistence(
        Guid id,
        Guid leaseId,
        Guid organizationId,
        Guid? tenantId,
        decimal amount,
        string status,
        string? notes,
        Guid? createdBy,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? paidAt,
        Guid? paidBy,
        DateTime? deletedAt,
        Guid? deletedBy)
    {
        return new DepositRefundEntity
        {
            Id = id,
            LeaseId = leaseId,
            OrganizationId = organizationId,
            TenantId = tenantId,
            Amount = amount,
            Status = status,
            Notes = notes,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            PaidAt = paidAt,
            PaidBy = paidBy,
            DeletedAt = deletedAt,
            DeletedBy = deletedBy
        };
    }

    public void MarkPaid(Guid paidByUserId, DateTime paidAtUtc)
    {
        if (!string.Equals(Status, "pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending refunds can be marked paid.");
        }

        Status = "paid";
        PaidBy = paidByUserId;
        PaidAt = paidAtUtc;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Forfeit(string reason)
    {
        if (!string.Equals(Status, "pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending refunds can be forfeited.");
        }

        Status = "forfeited";
        Notes = reason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
