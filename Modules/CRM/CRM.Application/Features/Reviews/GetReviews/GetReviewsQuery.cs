using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.GetReviews;

public sealed record ReviewListItemDto(Guid Id, Guid ViewingId, short Rating, string Content, DateTime CreatedAt);

public sealed record GetReviewsResult(PagedResult<ReviewListItemDto> Data);

public sealed record GetReviewsQuery(
    Guid OrganizationId,
    Guid? LeadId = null,
    Guid? ViewingId = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetReviewsResult>>;
