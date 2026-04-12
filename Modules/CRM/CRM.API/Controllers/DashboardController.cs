using CRM.Application.Features.Dashboard;
using CRM.Application.Features.Leads.GetLeads;
using CRM.Application.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/crm")]
public sealed class DashboardController(IAppSender sender) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<CrmDashboardSummaryDto>>> GetDashboard([FromQuery] Guid organizationId, CancellationToken cancellationToken)
    {
        var leads = await sender.Send(
            new GetLeadsQuery(organizationId, Paging: new PagingRequest(1, 200)),
            cancellationToken);
        if (leads.IsFailure)
        {
            return this.ToApiFailureResult<CrmDashboardSummaryDto>(leads.Error);
        }

        var items = leads.Value!.Data.Items;
        var payload = new CrmDashboardSummaryDto(
            items.Count(x => x.Status == "new"),
            items.Count(x => x.Status == "contacted"),
            items.Count(x => x.Status == "qualified"),
            items.Count(x => x.Status == "converted"),
            items.Count(x => x.Status == "disqualified"),
            DateTime.UtcNow);

        return Ok(ApiResponse<CrmDashboardSummaryDto>.Succeed(payload));
    }
}
