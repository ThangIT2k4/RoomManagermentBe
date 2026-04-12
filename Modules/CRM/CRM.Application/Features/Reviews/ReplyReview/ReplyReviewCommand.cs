using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.ReplyReview;

public sealed record ReplyReviewCommand(Guid ReviewId, string ReplyContent, Guid RepliedBy)
    : IAppRequest<Result<ReviewReplyDto>>;
