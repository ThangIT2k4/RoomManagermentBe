using CRM.Application.Features.Leads;
using CRM.Application.Features.UseCases;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Shared.Extensions;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDto>> CreateLead([FromBody] CreateLeadRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{leadId:guid}")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeadDto>> GetLeadById([FromRoute] Guid leadId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLeadByIdQuery(leadId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetLeadsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLeadsResult>> GetLeads([FromQuery] Guid organizationId, [FromQuery] string? search, [FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetLeadsQuery(organizationId, search, status, new PagingRequest(pageNumber, pageSize)),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPatch("{leadId:guid}/status")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDto>> UpdateLeadStatus([FromRoute] Guid leadId, [FromBody] UpdateLeadStatusRequest request, CancellationToken cancellationToken)
    {
        var command = request with { LeadId = leadId };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }
}
