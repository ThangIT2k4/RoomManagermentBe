namespace Lease.Application.Dtos;

public sealed record LeaseDto(
    Guid Id,
    Guid UnitId,
    Guid OrganizationId,
    string? LeaseNo,
    string Status,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RentAmount,
    decimal? DepositAmount,
    Guid? CycleId,
    int? PaymentDay,
    Guid? ParentLeaseId,
    Guid? BookingId,
    string? Notes);

public sealed record LeaseResidentDto(
    Guid Id,
    Guid LeaseId,
    Guid? UserId,
    string FullName,
    string? Phone,
    string? Email,
    string? IdNumber,
    string? Relationship,
    bool IsPrimary);

public sealed record LeaseServiceSetItemDto(Guid Id, Guid ServiceId, decimal Price, bool IsRequired);
public sealed record LeaseServiceSetDto(Guid Id, Guid OrganizationId, string Name, string? Description, IReadOnlyList<LeaseServiceSetItemDto> Items);
public sealed record PaymentCycleDto(Guid Id, Guid OrganizationId, string Name, int DurationMonths, int? DayOfMonth);
public sealed record MasterLeaseDto(Guid Id, Guid OrganizationId, Guid PropertyId, Guid LandlordUserId, string? ContractNo, DateOnly StartDate, DateOnly EndDate, decimal RentAmount, decimal? DepositAmount, int? PaymentDay, string Status, string? Notes);

public sealed record CreateLeaseFromBookingRequest(
    Guid BookingId,
    Guid UnitId,
    Guid TenantUserId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RentAmount,
    decimal? DepositAmount,
    Guid? CycleId,
    int? PaymentDay,
    string? Notes,
    Guid? LeaseServiceSetId);

public sealed record CreateManualLeaseRequest(
    Guid UnitId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RentAmount,
    decimal? DepositAmount,
    Guid? CycleId,
    int? PaymentDay,
    Guid? TenantUserId,
    string? TenantFullName,
    string? TenantPhone,
    string? TenantEmail,
    string? TenantIdNumber,
    string? Notes,
    Guid? LeaseServiceSetId);

public sealed record UpdateLeaseRequest(Guid LeaseId, DateOnly EndDate, Guid? CycleId, int? PaymentDay, string? Notes);
public sealed record RenewLeaseRequest(Guid OldLeaseId, DateOnly StartDate, DateOnly EndDate, decimal RentAmount, decimal? DepositAmount, Guid? CycleId, int? PaymentDay, string? Notes, Guid? LeaseServiceSetId);
public sealed record TerminateLeaseRequest(Guid LeaseId, DateOnly TerminationDate, string Reason, bool CreateRefund, decimal? RefundAmount, string? RefundNotes, string? Notes);
public sealed record AddResidentRequest(Guid LeaseId, Guid? UserId, string FullName, string? Phone, string? Email, string? IdNumber, string? Relationship);
public sealed record LinkResidentUserRequest(Guid LeaseId, Guid ResidentId, Guid UserId);
public sealed record ApplyServiceSetRequest(Guid LeaseId, Guid LeaseServiceSetId);
public sealed record UpsertPaymentCycleRequest(Guid? Id, string Name, int DurationMonths, int? DayOfMonth);
public sealed record UpsertMasterLeaseRequest(Guid? Id, Guid LandlordUserId, Guid PropertyId, string? ContractNo, DateOnly StartDate, DateOnly EndDate, decimal RentAmount, decimal? DepositAmount, int? PaymentDay, string? Notes);
public sealed record UpsertLeaseServiceSetRequest(Guid? Id, string Name, string? Description, IReadOnlyList<LeaseServiceSetItemWriteDto> Items);
public sealed record LeaseServiceSetItemWriteDto(Guid ServiceId, decimal Price, bool IsRequired);
