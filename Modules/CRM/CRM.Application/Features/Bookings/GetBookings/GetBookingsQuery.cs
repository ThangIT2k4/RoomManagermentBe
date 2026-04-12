using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Bookings.GetBookings;

public sealed record BookingListItemDto(
    Guid Id,
    Guid LeadId,
    decimal DepositAmount,
    string Currency,
    string Status,
    DateTime CreatedAt);

public sealed record GetBookingsResult(PagedResult<BookingListItemDto> Data);

public sealed record GetBookingsQuery(
    Guid OrganizationId,
    Guid? LeadId = null,
    string? Status = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetBookingsResult>>;
