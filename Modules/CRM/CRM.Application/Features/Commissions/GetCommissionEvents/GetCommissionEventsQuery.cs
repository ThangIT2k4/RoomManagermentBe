using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.GetCommissionEvents;

public sealed record CommissionEventListItemDto(
    Guid Id,
    Guid OrganizationId,
    Guid? AgentUserId,
    decimal Amount,
    string Status,
    DateTime CreatedAt);

public sealed record GetCommissionEventsResult(PagedResult<CommissionEventListItemDto> Data);

public sealed record GetCommissionEventsQuery(
    Guid OrganizationId,
    Guid? AgentUserId = null,
    string? Status = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetCommissionEventsResult>>;
