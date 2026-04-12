using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.GetViewings;

public sealed record ViewingListItemDto(Guid Id, Guid LeadId, DateTime ScheduledAt, string? Location, string Status);

public sealed record GetViewingsResult(PagedResult<ViewingListItemDto> Data);

public sealed record GetViewingsQuery(
    Guid OrganizationId,
    Guid? LeadId = null,
    Guid? AgentUserId = null,
    DateTime? From = null,
    DateTime? To = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetViewingsResult>>;
