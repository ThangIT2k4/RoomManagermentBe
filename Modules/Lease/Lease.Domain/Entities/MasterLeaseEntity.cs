namespace Lease.Domain.Entities;

public sealed class MasterLeaseEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid PropertyId { get; }
    public Guid LandlordUserId { get; }
    public string? ContractNo { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; private set; }
    public decimal RentAmount { get; }
    public decimal? DepositAmount { get; }
    public int? PaymentDay { get; }
    public string Status { get; private set; }

    private MasterLeaseEntity(Guid id, Guid organizationId, Guid propertyId, Guid landlordUserId, string? contractNo, DateOnly startDate, DateOnly endDate, decimal rentAmount, decimal? depositAmount, int? paymentDay, string status)
    {
        Id = id;
        OrganizationId = organizationId;
        PropertyId = propertyId;
        LandlordUserId = landlordUserId;
        ContractNo = contractNo;
        StartDate = startDate;
        EndDate = endDate;
        RentAmount = rentAmount;
        DepositAmount = depositAmount;
        PaymentDay = paymentDay;
        Status = status;
    }

    public static MasterLeaseEntity Create(Guid organizationId, Guid propertyId, Guid landlordUserId, DateOnly startDate, DateOnly endDate, decimal rentAmount, decimal? depositAmount, int? paymentDay, string? contractNo = null)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        if (propertyId == Guid.Empty) throw new ArgumentException("PropertyId is required.", nameof(propertyId));
        if (landlordUserId == Guid.Empty) throw new ArgumentException("LandlordUserId is required.", nameof(landlordUserId));
        if (endDate <= startDate) throw new ArgumentException("EndDate must be greater than StartDate.", nameof(endDate));
        if (rentAmount <= 0) throw new ArgumentException("RentAmount must be greater than 0.", nameof(rentAmount));
        if (paymentDay is < 1 or > 28) throw new ArgumentException("PaymentDay must be between 1 and 28.", nameof(paymentDay));
        return new MasterLeaseEntity(Guid.NewGuid(), organizationId, propertyId, landlordUserId, contractNo?.Trim(), startDate, endDate, rentAmount, depositAmount, paymentDay, LeaseStatuses.Active);
    }

    public void Terminate(DateOnly terminationDate)
    {
        if (Status != LeaseStatuses.Active) throw new InvalidOperationException("Only active master lease can be terminated.");
        if (terminationDate < StartDate) throw new ArgumentException("Termination date cannot be before start date.", nameof(terminationDate));
        EndDate = terminationDate;
        Status = LeaseStatuses.Terminated;
    }
}
