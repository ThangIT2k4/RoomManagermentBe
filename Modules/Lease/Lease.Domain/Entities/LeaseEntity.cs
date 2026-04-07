using Lease.Domain.Common;
using Lease.Domain.Enums;
using Lease.Domain.Events;
using Lease.Domain.Exceptions;
using Lease.Domain.ValueObjects;

namespace Lease.Domain.Entities;

public static class LeaseStatuses
{
    public const string Active = "active";
    public const string Terminated = "terminated";
    public const string Expired = "expired";
    public const string Renewed = "renewed";

    public static bool IsValid(string value) =>
        value is Active or Terminated or Expired or Renewed;
}

public sealed class LeaseEntity : AggregateRoot<Guid>
{
    public Guid UnitId { get; }
    public Guid OrganizationId { get; }
    public string? LeaseNo { get; }
    public decimal RentAmount { get; }
    public decimal? DepositAmount { get; }
    public Guid? CycleId { get; }
    public int? PaymentDay { get; }
    public string Status { get; private set; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; private set; }
    public Guid? ParentLeaseId { get; }
    public Guid? BookingId { get; }
    public string? Notes { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private LeaseEntity(
        Guid id,
        Guid unitId,
        Guid organizationId,
        string? leaseNo,
        decimal rentAmount,
        decimal? depositAmount,
        Guid? cycleId,
        int? paymentDay,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        Guid? parentLeaseId,
        Guid? bookingId,
        string? notes,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        UnitId = unitId;
        OrganizationId = organizationId;
        LeaseNo = leaseNo;
        RentAmount = rentAmount;
        DepositAmount = depositAmount;
        CycleId = cycleId;
        PaymentDay = paymentDay;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        ParentLeaseId = parentLeaseId;
        BookingId = bookingId;
        Notes = notes;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static LeaseEntity Create(
        Guid unitId,
        Guid organizationId,
        decimal rentAmount,
        DateOnly startDate,
        DateOnly endDate,
        decimal? depositAmount = null,
        Guid? cycleId = null,
        int? paymentDay = null,
        string status = LeaseStatuses.Active,
        string? leaseNo = null,
        Guid? parentLeaseId = null,
        Guid? bookingId = null,
        string? notes = null,
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

        if (rentAmount <= 0)
        {
            throw new LeaseValidationException("Lease.RentAmount.Invalid", "RentAmount must be greater than zero.");
        }

        _ = LeasePeriod.Create(startDate, endDate);

        if (paymentDay.HasValue)
        {
            _ = PaymentDayValue.Create(paymentDay.Value);
        }

        var normalizedStatus = NormalizeStatus(status);

        var entity = new LeaseEntity(
            Guid.NewGuid(),
            unitId,
            organizationId,
            leaseNo?.Trim(),
            rentAmount,
            depositAmount,
            cycleId,
            paymentDay,
            normalizedStatus,
            startDate,
            endDate,
            parentLeaseId,
            bookingId,
            notes?.Trim(),
            createdAt ?? DateTime.UtcNow,
            null);
        entity.AddDomainEvent(new LeaseActivatedEvent(
            entity.Id,
            entity.UnitId,
            entity.OrganizationId,
            entity.StartDate,
            entity.EndDate,
            entity.RentAmount,
            entity.DepositAmount,
            DateTimeOffset.UtcNow));
        return entity;
    }

    public static LeaseEntity FromPersistence(
        Guid id,
        Guid unitId,
        Guid organizationId,
        string? leaseNo,
        decimal rentAmount,
        decimal? depositAmount,
        Guid? cycleId,
        int? paymentDay,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        Guid? parentLeaseId,
        Guid? bookingId,
        string? notes,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new LeaseEntity(id, unitId, organizationId, leaseNo, rentAmount, depositAmount, cycleId, paymentDay, NormalizeStatus(status), startDate, endDate, parentLeaseId, bookingId, notes, createdAt, updatedAt);
    }

    public void Terminate(DateOnly terminationDate, string reason, DateTime at)
    {
        if (Status != LeaseStatuses.Active)
        {
            throw new InvalidLeaseStateException(InvalidLeaseStateException.CodeInvalidTransition, "Only active lease can be terminated.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required.", nameof(reason));
        }

        if (terminationDate < StartDate)
        {
            throw new ArgumentException("Termination date cannot be before start date.", nameof(terminationDate));
        }

        Status = LeaseStatuses.Terminated;
        EndDate = terminationDate;
        UpdatedAt = at;
        AddDomainEvent(new LeaseTerminatedEvent(Id, UnitId, OrganizationId, terminationDate, reason.Trim(), DateTimeOffset.UtcNow));
    }

    public void Expire(DateOnly asOfDate, DateTime at)
    {
        if (Status != LeaseStatuses.Active || EndDate >= asOfDate)
        {
            return;
        }

        Status = LeaseStatuses.Expired;
        UpdatedAt = at;
        AddDomainEvent(new LeaseExpiredEvent(Id, UnitId, OrganizationId, DateTimeOffset.UtcNow));
    }

    public void MarkRenewed(DateTime at)
    {
        if (Status != LeaseStatuses.Active && Status != LeaseStatuses.Expired)
        {
            throw new InvalidLeaseStateException(InvalidLeaseStateException.CodeInvalidTransition, "Only active or expired lease can be marked renewed.");
        }

        Status = LeaseStatuses.Renewed;
        UpdatedAt = at;
        AddDomainEvent(new LeaseUpdatedEvent(Id, UnitId, ["status", "updated_at"], DateTimeOffset.UtcNow));
    }

    private static string NormalizeStatus(string? status)
    {
        var normalized = status?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!LeaseStatuses.IsValid(normalized))
        {
            throw new InvalidLeaseStateException(InvalidLeaseStateException.CodeInvalidStatus, "Lease status is invalid.");
        }

        return normalized;
    }
}

