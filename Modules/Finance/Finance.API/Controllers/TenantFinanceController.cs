using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/tenant")]
public sealed class TenantFinanceController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpGet("invoices")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InvoiceDto>>>> ListInvoices(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return this.ApiBadRequest<IReadOnlyList<InvoiceDto>>("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.ListTenantInvoicesAsync(tenantUserId, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpGet("invoices/{invoiceId:guid}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return this.ApiBadRequest<InvoiceDto>("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.GetInvoiceForTenantAsync(tenantUserId, invoiceId, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentDto>>>> ListPayments(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return this.ApiBadRequest<IReadOnlyList<PaymentDto>>("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.ListTenantPaymentsAsync(tenantUserId, cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPost("invoices/{invoiceId:guid}/payment-request")]
    public async Task<ActionResult<ApiResponse<OnlinePaymentInitiationResult>>> PaymentRequest(
        Guid invoiceId,
        [FromBody] PaymentRequestApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetTenantUser(out var tenantUserId))
        {
            return this.ApiBadRequest<OnlinePaymentInitiationResult>("Header X-Tenant-User-Id is required.");
        }

        var result = await finance.InitiateOnlinePaymentAsync(
            tenantUserId,
            invoiceId,
            body.MethodKey,
            cancellationToken);

        return this.ToApiActionResult(result);
    }
}

public sealed record PaymentRequestApiRequest(string MethodKey);
