namespace CRM.Application.Features.Bookings;

public sealed record BookingDepositDto(
    Guid Id,
    Guid OrganizationId,
    Guid? LeadId,
    Guid? ViewingId,
    decimal Amount,
    string? DepositType,
    string PaymentStatus,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
