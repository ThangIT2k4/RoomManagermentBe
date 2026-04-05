using Finance.API;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/invoices")]
public sealed class InvoicesController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(
        [FromBody] CreateInvoiceApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var lines = body.Items.Select(i => new InvoiceItemLineDto(
            i.ItemType,
            i.Description,
            i.Quantity,
            i.UnitPrice,
            i.ServiceId,
            i.MeterReadingId,
            i.TicketLogId)).ToList();

        var result = await finance.CreateManualInvoiceAsync(
            orgId,
            userId,
            body.LeaseId,
            body.InvoiceDate,
            body.DueDate,
            body.Notes,
            lines,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{invoiceId:guid}")]
    public async Task<ActionResult<InvoiceDto>> UpdateDraft(
        Guid invoiceId,
        [FromBody] UpdateInvoiceApiRequest body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var lines = body.Items.Select(i => new InvoiceItemLineDto(
            i.ItemType,
            i.Description,
            i.Quantity,
            i.UnitPrice,
            i.ServiceId,
            i.MeterReadingId,
            i.TicketLogId)).ToList();

        var result = await finance.UpdateDraftInvoiceAsync(
            orgId,
            userId,
            invoiceId,
            body.DueDate,
            body.Notes,
            lines,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{invoiceId:guid}/publish")]
    public async Task<ActionResult<InvoiceDto>> Publish(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.PublishInvoiceAsync(orgId, invoiceId, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{invoiceId:guid}/cancel")]
    public async Task<ActionResult<InvoiceDto>> Cancel(
        Guid invoiceId,
        [FromBody] CancelInvoiceApiRequest? body,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.CancelInvoiceAsync(orgId, userId, invoiceId, body?.Reason, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{invoiceId:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await finance.GetInvoiceAsync(orgId, invoiceId, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<PagedInvoicesDto>> Search(
        [FromQuery] string? statuses,
        [FromQuery] Guid? leaseId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        IReadOnlyList<string>? statusList = null;
        if (!string.IsNullOrWhiteSpace(statuses))
        {
            statusList = statuses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        var result = await finance.SearchInvoicesAsync(
            orgId,
            statusList,
            leaseId,
            fromDate,
            toDate,
            search,
            page,
            perPage,
            cancellationToken);

        return result.ToActionResult();
    }
}

public sealed record CreateInvoiceApiRequest(
    Guid LeaseId,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyList<InvoiceItemLineApiRequest> Items);

public sealed record InvoiceItemLineApiRequest(
    string ItemType,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    Guid? ServiceId,
    Guid? MeterReadingId,
    Guid? TicketLogId);

public sealed record UpdateInvoiceApiRequest(
    DateOnly DueDate,
    string? Notes,
    IReadOnlyList<InvoiceItemLineApiRequest> Items);

public sealed record CancelInvoiceApiRequest(string? Reason);
