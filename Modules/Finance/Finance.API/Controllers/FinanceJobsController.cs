using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/internal/jobs")]
public sealed class FinanceJobsController(IFinanceScheduledJobPublisher jobs) : ControllerBase
{
    [HttpPost("auto-invoices")]
    public async Task<ActionResult<ApiResponse<FinanceEnqueueAutoInvoicesResponse>>> EnqueueAutoInvoices(
        [FromBody] AutoInvoiceJobApiRequest body,
        CancellationToken cancellationToken)
    {
        var result = await jobs.PublishAutoInvoiceGenerationAsync(body.RunDate, cancellationToken);
        return this.ToApiVoidActionResult<FinanceEnqueueAutoInvoicesResponse>(result);
    }

    [HttpPost("overdue-sweep")]
    public async Task<ActionResult<ApiResponse<FinanceEnqueueOverdueSweepResponse>>> EnqueueOverdueSweep(
        [FromBody] OverdueSweepJobApiRequest body,
        CancellationToken cancellationToken)
    {
        var result = await jobs.PublishOverdueSweepAsync(body.AsOfDate, cancellationToken);
        return this.ToApiVoidActionResult<FinanceEnqueueOverdueSweepResponse>(result);
    }
}

public sealed record AutoInvoiceJobApiRequest(DateOnly RunDate);

public sealed record OverdueSweepJobApiRequest(DateOnly AsOfDate);
