using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    public Task<Result<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        => crm.CreateReviewAsync(request, cancellationToken);
}
