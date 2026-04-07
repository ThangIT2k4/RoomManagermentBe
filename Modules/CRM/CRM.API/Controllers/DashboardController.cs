using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/crm")]
public sealed class DashboardController(ICrmApplicationService crmService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<object>> GetDashboard([FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        var leads = await crmService.GetLeadsAsync(new GetLeadsQuery(organizationId, Paging: new PagingRequest(1, 200)), cancellationToken);
        if (leads.IsFailure)
        {
            return BadRequest(new { error = leads.Error });
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
