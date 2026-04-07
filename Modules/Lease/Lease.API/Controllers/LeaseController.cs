using Lease.API;
using Lease.Application.Dtos;
using Lease.Application.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Messaging.Contracts.Messages;

namespace Lease.Api.Controllers;

[ApiController]
[Route("api/lease")]
public sealed class LeaseController(ILeaseApplicationService service, IBus bus) : ControllerBase
{
    [HttpPost("leases/from-booking")]
    public async Task<ActionResult<LeaseDto>> CreateFromBooking([FromBody] CreateLeaseFromBookingRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.CreateFromBookingAsync(orgId, userId, request, cancellationToken));
    }

    [HttpPost("leases")]
    public async Task<ActionResult<LeaseDto>> CreateManual([FromBody] CreateManualLeaseRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.CreateManualAsync(orgId, userId, request, cancellationToken));
    }

    [HttpGet("leases")]
    public async Task<ActionResult<IReadOnlyList<LeaseDto>>> Search([FromQuery] string? statuses, [FromQuery] Guid? unitId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return BadRequest(new { error = pagingError });
        }

        return Ok(await service.SearchLeasesAsync(orgId, statuses, unitId, search, normalizedPage, normalizedPerPage, cancellationToken));
    }

    [HttpGet("leases/{leaseId:guid}")]
    public async Task<ActionResult<LeaseDto>> Detail(Guid leaseId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.GetLeaseByIdAsync(orgId, leaseId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("tenant/leases")]
    public async Task<ActionResult<IReadOnlyList<LeaseDto>>> TenantLeases([FromQuery] Guid? userId, CancellationToken cancellationToken)
    {
        var id = userId ?? (HttpContext.TryGetOrgAndUser(out _, out var actor) ? actor : Guid.Empty);
        if (id == Guid.Empty) return BadRequest("User id is required.");
        return Ok(await service.GetTenantLeasesAsync(id, cancellationToken));
    }

    [HttpPut("leases/{leaseId:guid}")]
    public async Task<ActionResult<LeaseDto>> Update(Guid leaseId, [FromBody] UpdateLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var updated = await service.UpdateLeaseAsync(orgId, userId, new UpdateLeaseRequest(leaseId, body.EndDate, body.CycleId, body.PaymentDay, body.Notes), cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("leases/{leaseId:guid}/renew")]
    public async Task<ActionResult<LeaseDto>> Renew(Guid leaseId, [FromBody] RenewLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.RenewLeaseAsync(orgId, userId, new RenewLeaseRequest(leaseId, body.StartDate, body.EndDate, body.RentAmount, body.DepositAmount, body.CycleId, body.PaymentDay, body.Notes, body.LeaseServiceSetId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("leases/{leaseId:guid}/terminate")]
    public async Task<ActionResult<LeaseDto>> Terminate(Guid leaseId, [FromBody] TerminateLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.TerminateLeaseAsync(orgId, userId, new TerminateLeaseRequest(leaseId, body.TerminationDate, body.Reason, body.CreateRefund, body.RefundAmount, body.RefundNotes, body.Notes), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("leases/{leaseId:guid}/residents")]
    public async Task<ActionResult<IReadOnlyList<LeaseResidentDto>>> Residents(Guid leaseId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetResidentsAsync(orgId, leaseId, cancellationToken));
    }

    [HttpPost("leases/{leaseId:guid}/residents")]
    public async Task<ActionResult<LeaseResidentDto>> AddResident(Guid leaseId, [FromBody] AddResidentBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.AddResidentAsync(orgId, userId, new AddResidentRequest(leaseId, body.UserId, body.FullName, body.Phone, body.Email, body.IdNumber, body.Relationship), cancellationToken));
    }

    [HttpDelete("leases/{leaseId:guid}/residents/{residentId:guid}")]
    public async Task<IActionResult> RemoveResident(Guid leaseId, Guid residentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.RemoveResidentAsync(orgId, userId, leaseId, residentId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPost("leases/{leaseId:guid}/residents/{residentId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimary(Guid leaseId, Guid residentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.SetPrimaryResidentAsync(orgId, userId, leaseId, residentId, cancellationToken) ? Ok() : NotFound();
    }

    [HttpPost("leases/{leaseId:guid}/residents/{residentId:guid}/link-user")]
    public async Task<ActionResult<LeaseResidentDto>> LinkUser(Guid leaseId, Guid residentId, [FromBody] LinkUserBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.LinkResidentUserAsync(orgId, userId, new LinkResidentUserRequest(leaseId, residentId, body.UserId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("leases/{leaseId:guid}/apply-service-set")]
    public async Task<IActionResult> ApplyServiceSet(Guid leaseId, [FromBody] ApplyServiceSetBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.ApplyServiceSetAsync(orgId, userId, new ApplyServiceSetRequest(leaseId, body.LeaseServiceSetId), cancellationToken) ? Ok() : NotFound();
    }

    [HttpGet("lease-service-sets")]
    public async Task<ActionResult<IReadOnlyList<LeaseServiceSetDto>>> GetServiceSets(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetServiceSetsAsync(orgId, cancellationToken));
    }

    [HttpGet("lease-service-sets/{id:guid}")]
    public async Task<ActionResult<LeaseServiceSetDto>> GetServiceSetById(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.GetServiceSetByIdAsync(orgId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("lease-service-sets")]
    public async Task<ActionResult<LeaseServiceSetDto>> UpsertServiceSet([FromBody] UpsertLeaseServiceSetRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertServiceSetAsync(orgId, userId, request, cancellationToken));
    }

    [HttpGet("payment-cycles")]
    public async Task<ActionResult<IReadOnlyList<PaymentCycleDto>>> GetPaymentCycles(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetPaymentCyclesAsync(orgId, cancellationToken));
    }

    [HttpPost("payment-cycles")]
    public async Task<ActionResult<PaymentCycleDto>> UpsertPaymentCycle([FromBody] UpsertPaymentCycleRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertPaymentCycleAsync(orgId, userId, request, cancellationToken));
    }

    [HttpDelete("payment-cycles/{id:guid}")]
    public async Task<IActionResult> DeletePaymentCycle(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.DeletePaymentCycleAsync(orgId, userId, id, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("master-leases")]
    public async Task<ActionResult<IReadOnlyList<MasterLeaseDto>>> GetMasterLeases(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetMasterLeasesAsync(orgId, cancellationToken));
    }

    [HttpPost("master-leases")]
    public async Task<ActionResult<MasterLeaseDto>> UpsertMasterLease([FromBody] UpsertMasterLeaseRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertMasterLeaseAsync(orgId, userId, request, cancellationToken));
    }

    [HttpPost("master-leases/{id:guid}/terminate")]
    public async Task<ActionResult<MasterLeaseDto>> TerminateMasterLease(Guid id, [FromBody] TerminateMasterLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.TerminateMasterLeaseAsync(orgId, userId, id, body.TerminationDate, body.Reason, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("internal/jobs/expiring-check")]
    public async Task<ActionResult<object>> RunExpiringCheck([FromQuery] DateOnly? asOfDate, CancellationToken cancellationToken)
    {
        var runDate = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        await bus.Publish(new LeaseExpiringCheckRequested { AsOfDate = runDate, CorrelationId = Guid.NewGuid() }, cancellationToken);
        return Accepted(new { queued = true, asOfDate = runDate });
    }

    [HttpPost("internal/jobs/expiry-sweep")]
    public async Task<ActionResult<object>> RunExpirySweep([FromQuery] DateOnly? asOfDate, CancellationToken cancellationToken)
    {
        var runDate = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        await bus.Publish(new LeaseExpirySweepRequested { AsOfDate = runDate, CorrelationId = Guid.NewGuid() }, cancellationToken);
        return Accepted(new { queued = true, asOfDate = runDate });
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

public sealed record UpdateLeaseBody(DateOnly EndDate, Guid? CycleId, int? PaymentDay, string? Notes);
public sealed record RenewLeaseBody(DateOnly StartDate, DateOnly EndDate, decimal RentAmount, decimal? DepositAmount, Guid? CycleId, int? PaymentDay, string? Notes, Guid? LeaseServiceSetId);
public sealed record TerminateLeaseBody(DateOnly TerminationDate, string Reason, bool CreateRefund, decimal? RefundAmount, string? RefundNotes, string? Notes);
public sealed record AddResidentBody(Guid? UserId, string FullName, string? Phone, string? Email, string? IdNumber, string? Relationship);
public sealed record LinkUserBody(Guid UserId);
public sealed record ApplyServiceSetBody(Guid LeaseServiceSetId);
public sealed record TerminateMasterLeaseBody(DateOnly TerminationDate, string Reason);
