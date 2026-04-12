using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class PaymentsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost("invoices/{invoiceId:guid}/payments")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> RecordManual(
        Guid invoiceId,
        [FromBody] RecordManualPaymentApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<InvoiceDto>("Headers X-Organization-Id and X-User-Id are required.");
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
        return this.ToApiActionResult(result);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<ApiResponse<PagedPaymentsDto>>> Search(
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
            return this.ApiBadRequest<PagedPaymentsDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return this.ApiBadRequest<PagedPaymentsDto>(pagingError ?? "Invalid paging.");
        }

        var result = await finance.SearchPaymentsAsync(
            orgId,
            fromPaidAtUtc,
            toPaidAtUtc,
            methodId,
            status,
            normalizedPage,
            normalizedPerPage,
            cancellationToken);

        return this.ToApiActionResult(result);
    }

    private static bool TryNormalizePaging(int page, int perPage, out int normalizedPage, out int normalizedPerPage, out string? error)
    {
        normalizedPage = page;
        normalizedPerPage = perPage;
        error = null;

        if (page < 1)
        {
            error = "Page must be greater than or equal to 1.";
            return false;
        }

        if (perPage < 1 || perPage > 200)
        {
            error = "PerPage must be between 1 and 200.";
            return false;
        }

        return true;
    }
}

public sealed record RecordManualPaymentApiRequest(
    Guid MethodId,
    decimal Amount,
    DateTime PaidAtUtc,
    string? ReferenceCode,
    string? Note);
