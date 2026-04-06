namespace Lease.Domain.Entities;

public sealed class LeaseEntity
{
    public Guid Id { get; }
    public Guid UnitId { get; }
    public Guid OrganizationId { get; }
    public string? LeaseNo { get; }
    public decimal RentAmount { get; }
    public string Status { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private LeaseEntity(
        Guid id,
        Guid unitId,
        Guid organizationId,
        string? leaseNo,
        decimal rentAmount,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        UnitId = unitId;
        OrganizationId = organizationId;
        LeaseNo = leaseNo;
        RentAmount = rentAmount;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static LeaseEntity Create(
        Guid unitId,
        Guid organizationId,
        decimal rentAmount,
        DateOnly startDate,
        DateOnly endDate,
        string status = "draft",
        string? leaseNo = null,
        DateTime? createdAt = null)
    {
        if (unitId == Guid.Empty)
        {
            throw new ArgumentException("UnitId is required.", nameof(unitId));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status is required.", nameof(status));
        }

        return new LeaseEntity(Guid.NewGuid(), unitId, organizationId, leaseNo?.Trim(), rentAmount, status.Trim(), startDate, endDate, createdAt ?? DateTime.UtcNow, null);
    }

    public static LeaseEntity FromPersistence(
        Guid id,
        Guid unitId,
        Guid organizationId,
        string? leaseNo,
        decimal rentAmount,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new LeaseEntity(id, unitId, organizationId, leaseNo, rentAmount, status ?? string.Empty, startDate, endDate, createdAt, updatedAt);
    }
}

