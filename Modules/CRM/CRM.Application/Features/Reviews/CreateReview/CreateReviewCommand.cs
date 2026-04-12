using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Reviews.CreateReview;

public sealed record CreateReviewCommand(Guid ViewingId, short Rating, string Content, Guid CreatedBy)
    : IAppRequest<Result<ReviewDto>>;
