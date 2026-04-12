using RoomManagerment.Shared.Extensions;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class FinanceReportsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpGet("debt-summary")]
    public async Task<ActionResult<DebtSummaryDto>> DebtSummary(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.GetDebtSummaryAsync(orgId, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<IReadOnlyList<RevenueMonthDto>>> Revenue(
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.GetRevenueByMonthAsync(orgId, year, cancellationToken);
        return result.ToActionResult(this);
    }
}
