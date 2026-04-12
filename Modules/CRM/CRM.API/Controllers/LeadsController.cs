using CRM.Application.Features.Leads;
using CRM.Application.Features.Leads.CreateLead;
using CRM.Application.Features.Leads.GetLeadById;
using CRM.Application.Features.Leads.GetLeads;
using CRM.Application.Features.Leads.UpdateLeadStatus;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(IAppSender sender) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(ApiResponse<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LeadDto>>> CreateLead([FromBody] CreateLeadRequest request, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(request, cancellationToken));

    [HttpGet("{leadId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LeadDto>>> GetLeadById([FromRoute] Guid leadId, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(new GetLeadByIdQuery(leadId), cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetLeadsResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetLeadsResult>>> GetLeads(
        [FromQuery] Guid organizationId,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToApiActionResult(await sender.Send(
            new GetLeadsQuery(organizationId, search, status, new PagingRequest(pageNumber, pageSize)),
            cancellationToken));

    [HttpPatch("{leadId:guid}/status")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(ApiResponse<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LeadDto>>> UpdateLeadStatus([FromRoute] Guid leadId, [FromBody] UpdateLeadStatusRequest request, CancellationToken cancellationToken)
        => this.ToApiActionResult(await sender.Send(request with { LeadId = leadId }, cancellationToken));
}
