using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.ReplyReview;

public sealed class ReplyReviewCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<ReplyReviewCommand, Result<ReviewReplyDto>>
{
    public Task<Result<ReviewReplyDto>> Handle(ReplyReviewCommand request, CancellationToken cancellationToken)
        => crm.ReplyReviewAsync(request, cancellationToken);
}
