using CRM.Application.Features.Reviews;
using CRM.Application.Features.Reviews.CreateReview;
using CRM.Application.Features.Reviews.GetReviews;
using CRM.Application.Features.Reviews.ReplyReview;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController(IAppSender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Create([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command, cancellationToken));

    [HttpPost("{reviewId:guid}/replies")]
    public async Task<ActionResult<ApiResponse<ReviewReplyDto>>> Reply([FromRoute] Guid reviewId, [FromBody] ReplyReviewCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command with { ReviewId = reviewId }, cancellationToken));

    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetReviewsResult>>> List(
        [FromQuery] Guid organizationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToApiActionResult(await sender.Send(
            new GetReviewsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)),
            cancellationToken));
}
