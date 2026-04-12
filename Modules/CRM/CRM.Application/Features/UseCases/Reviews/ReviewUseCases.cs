using MediatR;

namespace CRM.Application.Features.UseCases;

public sealed record CreateReviewCommand(Guid ViewingId, short Rating, string Content, Guid CreatedBy)
    : IRequest<Result<ReviewDto>>;
public sealed record UpdateReviewCommand(Guid ReviewId, short Rating, string Content, Guid UpdatedBy);
public sealed record DeleteReviewCommand(Guid ReviewId, Guid RequestedBy);
public sealed record GetReviewsQuery(Guid OrganizationId, Guid? LeadId = null, Guid? ViewingId = null, PagingRequest? Paging = null)
    : IRequest<Result<GetReviewsResult>>;
public sealed record GetReviewByIdQuery(Guid ReviewId);
public sealed record ReplyReviewCommand(Guid ReviewId, string ReplyContent, Guid RepliedBy)
    : IRequest<Result<ReviewReplyDto>>;
public sealed record UpdateReviewReplyCommand(Guid ReplyId, string ReplyContent, Guid UpdatedBy);
public sealed record DeleteReviewReplyCommand(Guid ReplyId, Guid RequestedBy);
