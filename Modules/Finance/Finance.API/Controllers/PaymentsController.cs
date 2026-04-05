using Finance.API;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class PaymentsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost("invoices/{invoiceId:guid}/payments")]
    public async Task<ActionResult<InvoiceDto>> RecordManual(
        Guid invoiceId,
        [FromBody] RecordManualPaymentApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new RecordManualPaymentRequest(
            orgId,
            userId,
            invoiceId,
            body.MethodId,
            body.Amount,
            body.PaidAtUtc,
            body.ReferenceCode,
            body.Note);

        var result = await finance.RecordManualPaymentAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("payments")]
    public async Task<ActionResult<PagedPaymentsDto>> Search(
        [FromQuery] DateTime? fromPaidAtUtc,
        [FromQuery] DateTime? toPaidAtUtc,
        [FromQuery] Guid? methodId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.SearchPaymentsAsync(
            orgId,
            fromPaidAtUtc,
            toPaidAtUtc,
            methodId,
            status,
            page,
            perPage,
            cancellationToken);

        return result.ToActionResult();
    }
}

public sealed record RecordManualPaymentApiRequest(
    Guid MethodId,
    decimal Amount,
    DateTime PaidAtUtc,
    string? ReferenceCode,
    string? Note);
