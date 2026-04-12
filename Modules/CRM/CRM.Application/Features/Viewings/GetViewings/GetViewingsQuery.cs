using CRM.Application.Features.Shared;
using CRM.Application.Features.Viewings;
using MediatR;

namespace CRM.Application.Features.Viewings.GetViewings;

public sealed record ViewingListItemDto(Guid Id, Guid LeadId, DateTime ScheduledAt, string? Location, string Status);

public sealed record GetViewingsResult(PagedResult<ViewingListItemDto> Data);

/// <summary>Query: danh sách viewing (đọc).</summary>
public sealed record GetViewingsQuery(
    Guid OrganizationId,
    Guid? LeadId = null,
    Guid? AgentUserId = null,
    DateTime? From = null,
    DateTime? To = null,
    PagingRequest? Paging = null)
    : IRequest<Result<GetViewingsResult>>;
