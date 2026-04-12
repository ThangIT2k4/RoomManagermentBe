using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.GetReviews;

public sealed class GetReviewsQueryHandler(ICrmApplicationService crm)
    : IAppRequestHandler<GetReviewsQuery, Result<GetReviewsResult>>
{
    public Task<Result<GetReviewsResult>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        => crm.GetReviewsAsync(request, cancellationToken);
}
