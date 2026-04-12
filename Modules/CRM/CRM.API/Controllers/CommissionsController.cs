using RoomManagerment.Shared.Extensions;
using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/commissions")]
public sealed class CommissionsController(ICrmApplicationService crmService) : ControllerBase
{
    [HttpPost("policies")]
    public async Task<ActionResult<CommissionPolicyDto>> CreatePolicy([FromBody] CreateCommissionPolicyCommand command, CancellationToken cancellationToken)
        => (await crmService.CreateCommissionPolicyAsync(command, cancellationToken)).ToActionResult(this);

    [HttpPost("events/{eventId:guid}/approve")]
    public async Task<ActionResult<CommissionEventDto>> Approve([FromRoute] Guid eventId, [FromBody] ApproveCommissionCommand command, CancellationToken cancellationToken)
        => (await crmService.ApproveCommissionAsync(command with { CommissionEventId = eventId }, cancellationToken)).ToActionResult(this);

    [HttpGet("policies")]
    public async Task<ActionResult<GetCommissionPoliciesResult>> Policies([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await crmService.GetCommissionPoliciesAsync(new GetCommissionPoliciesQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken)).ToActionResult(this);

    [HttpGet("events")]
    public async Task<ActionResult<GetCommissionEventsResult>> Events([FromQuery] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await crmService.GetCommissionEventsAsync(new GetCommissionEventsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)), cancellationToken)).ToActionResult(this);
}
