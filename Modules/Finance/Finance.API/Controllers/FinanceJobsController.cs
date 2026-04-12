using RoomManagerment.Shared.Extensions;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/internal/jobs")]
public sealed class FinanceJobsController(IFinanceScheduledJobPublisher jobs) : ControllerBase
{
    [HttpPost("auto-invoices")]
    public async Task<IActionResult> EnqueueAutoInvoices(
        [FromBody] AutoInvoiceJobApiRequest body,
        CancellationToken cancellationToken)
    {
        var result = await jobs.PublishAutoInvoiceGenerationAsync(body.RunDate, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("overdue-sweep")]
    public async Task<IActionResult> EnqueueOverdueSweep(
        [FromBody] OverdueSweepJobApiRequest body,
        CancellationToken cancellationToken)
    {
        var result = await jobs.PublishOverdueSweepAsync(body.AsOfDate, cancellationToken);
        return result.ToActionResult(this);
    }
}

public sealed record AutoInvoiceJobApiRequest(DateOnly RunDate);

public sealed record OverdueSweepJobApiRequest(DateOnly AsOfDate);
