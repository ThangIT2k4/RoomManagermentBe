using MediatR;

namespace CRM.Application.Features.UseCases;

public sealed record CreateBookingCommand(Guid LeadId, decimal DepositAmount, string Currency, DateTime ExpiredAt, Guid RequestedBy)
    : IRequest<Result<BookingDepositDto>>;
public sealed record UpdateBookingCommand(Guid BookingId, decimal? DepositAmount = null, DateTime? ExpiredAt = null, string? Note = null, Guid? RequestedBy = null);
public sealed record CancelBookingCommand(Guid BookingId, string Reason, Guid RequestedBy);
public sealed record GetBookingsQuery(Guid OrganizationId, Guid? LeadId = null, string? Status = null, PagingRequest? Paging = null)
    : IRequest<Result<GetBookingsResult>>;
public sealed record GetBookingByIdQuery(Guid BookingId);
public sealed record PayBookingCommand(Guid BookingId, decimal PaidAmount, DateTime PaidAt, string? PaymentMethod, Guid RequestedBy)
    : IRequest<Result<BookingDepositDto>>;
public sealed record ApproveBookingCommand(Guid BookingId, Guid ApprovedBy, DateTime? ApprovedAt = null)
    : IRequest<Result<BookingDepositDto>>;
public sealed record ExpireBookingCommand(Guid BookingId, DateTime? ExpiredAt = null, string? Reason = null);
public sealed record AttachPaymentToBookingCommand(Guid BookingId, string PaymentReference, decimal Amount, DateTime PaidAt, Guid RequestedBy);
