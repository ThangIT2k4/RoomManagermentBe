using CRM.API.Common;
using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController(ICrmApplicationService crmService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        => (await crmService.CreateReviewAsync(command, cancellationToken)).ToActionResult();

    [HttpPost("{reviewId:guid}/replies")]
    public async Task<ActionResult<ReviewReplyDto>> Reply([FromRoute] Guid reviewId, [FromBody] ReplyReviewCommand command, CancellationToken cancellationToken)
        => (await crmService.ReplyReviewAsync(command with { ReviewId = reviewId }, cancellationToken)).ToActionResult();

    [HttpGet]
    public async Task<ActionResult<GetReviewsResult>> List([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await crmService.GetReviewsAsync(new GetReviewsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken)).ToActionResult();
}
