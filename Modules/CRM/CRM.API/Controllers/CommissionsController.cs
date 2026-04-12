using CRM.Application.Features.Commissions;
using CRM.Application.Features.Commissions.ApproveCommission;
using CRM.Application.Features.Commissions.CreateCommissionPolicy;
using CRM.Application.Features.Commissions.GetCommissionEvents;
using CRM.Application.Features.Commissions.GetCommissionPolicies;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/commissions")]
public sealed class CommissionsController(IAppSender sender) : ControllerBase
{
    [HttpPost("policies")]
    public async Task<ActionResult<ApiResponse<CommissionPolicyDto>>> CreatePolicy([FromBody] CreateCommissionPolicyCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command, cancellationToken));

    [HttpPost("events/{eventId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<CommissionEventDto>>> Approve([FromRoute] Guid eventId, [FromBody] ApproveCommissionCommand command, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(command with { CommissionEventId = eventId }, cancellationToken));

    [HttpGet("policies")]
    public async Task<ActionResult<ApiResponse<GetCommissionPoliciesResult>>> Policies(
        [FromQuery] Guid organizationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToApiActionResult(await sender.Send(
            new GetCommissionPoliciesQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)),
            cancellationToken));

    [HttpGet("events")]
    public async Task<ActionResult<ApiResponse<GetCommissionEventsResult>>> Events(
        [FromQuery] Guid organizationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToApiActionResult(await sender.Send(
            new GetCommissionEventsQuery(organizationId, Paging: new PagingRequest(pageNumber, pageSize)),
            cancellationToken));
}
