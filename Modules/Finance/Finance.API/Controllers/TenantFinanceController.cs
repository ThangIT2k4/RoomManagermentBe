using RoomManagerment.Shared.Extensions;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/tenant")]
public sealed class TenantFinanceController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpGet("invoices")]
    public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> ListInvoices(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return BadRequest("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.ListTenantInvoicesAsync(tenantUserId, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("invoices/{invoiceId:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return BadRequest("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.GetInvoiceForTenantAsync(tenantUserId, invoiceId, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> ListPayments(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return BadRequest("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.ListTenantPaymentsAsync(tenantUserId, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("invoices/{invoiceId:guid}/payment-request")]
    public async Task<ActionResult<OnlinePaymentInitiationResult>> PaymentRequest(
        Guid invoiceId,
        [FromBody] PaymentRequestApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return BadRequest("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.InitiateOnlinePaymentAsync(
            tenantUserId,
            invoiceId,
            body.MethodKey,
            cancellationToken);

        return result.ToActionResult(this);
    }
}

public sealed record PaymentRequestApiRequest(string MethodKey);
