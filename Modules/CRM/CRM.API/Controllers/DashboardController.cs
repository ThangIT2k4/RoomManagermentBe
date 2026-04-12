using CRM.Application.Features.Leads.GetLeads;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/crm")]
public sealed class DashboardController(IAppSender sender) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<object>> GetDashboard([FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        var leads = await sender.Send(
            new GetLeadsQuery(organizationId, Paging: new PagingRequest(1, 200)),
            cancellationToken);
        if (leads.IsFailure)
        {
            return this.ToActionResult(leads);
        }

        var items = leads.Value!.Data.Items;
        var payload = new
        {
            leadsNew = items.Count(x => x.Status == "new"),
            leadsContacted = items.Count(x => x.Status == "contacted"),
            leadsQualified = items.Count(x => x.Status == "qualified"),
            leadsConverted = items.Count(x => x.Status == "converted"),
            leadsLost = items.Count(x => x.Status == "disqualified"),
            fetchedAt = DateTime.UtcNow
        };

        return Ok(payload);
    }
}
