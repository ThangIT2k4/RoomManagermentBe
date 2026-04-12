using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class FinanceReportsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpGet("debt-summary")]
    public async Task<ActionResult<ApiResponse<DebtSummaryDto>>> DebtSummary(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<DebtSummaryDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.GetDebtSummaryAsync(orgId, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RevenueMonthDto>>>> Revenue(
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<RevenueMonthDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.GetRevenueByMonthAsync(orgId, year, cancellationToken);
        return this.ToApiActionResult(result);
    }
}
