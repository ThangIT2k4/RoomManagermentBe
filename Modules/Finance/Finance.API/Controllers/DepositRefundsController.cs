using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class DepositRefundsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost("leases/{leaseId:guid}/deposit-refunds")]
    public async Task<ActionResult<ApiResponse<DepositRefundDto>>> Create(
        Guid leaseId,
        [FromBody] CreateDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<DepositRefundDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new CreateDepositRefundRequest(orgId, userId, leaseId, body.Amount, body.Notes);
        var result = await finance.CreateDepositRefundAsync(request, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPost("deposit-refunds/{refundId:guid}/confirm-paid")]
    public async Task<ActionResult<ApiResponse<DepositRefundDto>>> ConfirmPaid(
        Guid refundId,
        [FromBody] ConfirmDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<DepositRefundDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new ConfirmDepositRefundRequest(orgId, userId, body.PaidAtUtc, body.ReferenceCode);
        var result = await finance.ConfirmDepositRefundPaidAsync(orgId, refundId, request, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPost("deposit-refunds/{refundId:guid}/forfeit")]
    public async Task<ActionResult<ApiResponse<DepositRefundDto>>> Forfeit(
        Guid refundId,
        [FromBody] ForfeitDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<DepositRefundDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new ForfeitDepositRefundRequest(orgId, userId, body.Reason);
        var result = await finance.ForfeitDepositRefundAsync(orgId, refundId, request, cancellationToken);
        return this.ToApiActionResult(result);
    }
}

public sealed record CreateDepositRefundApiRequest(decimal Amount, string? Notes);

public sealed record ConfirmDepositRefundApiRequest(DateTime PaidAtUtc, string? ReferenceCode);

public sealed record ForfeitDepositRefundApiRequest(string Reason);
