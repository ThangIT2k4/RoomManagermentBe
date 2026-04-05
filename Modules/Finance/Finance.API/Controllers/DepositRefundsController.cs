using Finance.API;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class DepositRefundsController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost("leases/{leaseId:guid}/deposit-refunds")]
    public async Task<ActionResult<DepositRefundDto>> Create(
        Guid leaseId,
        [FromBody] CreateDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new CreateDepositRefundRequest(orgId, userId, leaseId, body.Amount, body.Notes);
        var result = await finance.CreateDepositRefundAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("deposit-refunds/{refundId:guid}/confirm-paid")]
    public async Task<ActionResult<DepositRefundDto>> ConfirmPaid(
        Guid refundId,
        [FromBody] ConfirmDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new ConfirmDepositRefundRequest(orgId, userId, body.PaidAtUtc, body.ReferenceCode);
        var result = await finance.ConfirmDepositRefundPaidAsync(orgId, refundId, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("deposit-refunds/{refundId:guid}/forfeit")]
    public async Task<ActionResult<DepositRefundDto>> Forfeit(
        Guid refundId,
        [FromBody] ForfeitDepositRefundApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var request = new ForfeitDepositRefundRequest(orgId, userId, body.Reason);
        var result = await finance.ForfeitDepositRefundAsync(orgId, refundId, request, cancellationToken);
        return result.ToActionResult();
    }
}

public sealed record CreateDepositRefundApiRequest(decimal Amount, string? Notes);

public sealed record ConfirmDepositRefundApiRequest(DateTime PaidAtUtc, string? ReferenceCode);

public sealed record ForfeitDepositRefundApiRequest(string Reason);
