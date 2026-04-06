namespace CRM.Application.Features.UseCases;

public sealed record CreateReviewCommand(Guid ViewingId, short Rating, string Content, Guid CreatedBy);
public sealed record UpdateReviewCommand(Guid ReviewId, short Rating, string Content, Guid UpdatedBy);
public sealed record DeleteReviewCommand(Guid ReviewId, Guid RequestedBy);
public sealed record GetReviewsQuery(Guid OrganizationId, Guid? LeadId = null, Guid? ViewingId = null, PagingRequest? Paging = null);
public sealed record GetReviewByIdQuery(Guid ReviewId);
public sealed record ReplyReviewCommand(Guid ReviewId, string ReplyContent, Guid RepliedBy);
public sealed record UpdateReviewReplyCommand(Guid ReplyId, string ReplyContent, Guid UpdatedBy);
public sealed record DeleteReviewReplyCommand(Guid ReplyId, Guid RequestedBy);
