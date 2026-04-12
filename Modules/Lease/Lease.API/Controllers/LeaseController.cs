using Lease.API;
using Lease.Application.Dtos;
using Lease.Application.Features.Leases.CreateFromBooking;
using Lease.Application.Features.Leases.CreateManual;
using Lease.Application.Features.Leases.GetLeaseById;
using Lease.Application.Features.Leases.GetTenantLeases;
using Lease.Application.Features.Leases.RenewLease;
using Lease.Application.Features.Leases.SearchLeases;
using Lease.Application.Features.Leases.TerminateLease;
using Lease.Application.Features.Leases.UpdateLease;
using Lease.Application.Features.MasterLeases.GetMasterLeases;
using Lease.Application.Features.MasterLeases.TerminateMasterLease;
using Lease.Application.Features.MasterLeases.UpsertMasterLease;
using Lease.Application.Features.PaymentCycles.DeletePaymentCycle;
using Lease.Application.Features.PaymentCycles.GetPaymentCycles;
using Lease.Application.Features.PaymentCycles.UpsertPaymentCycle;
using Lease.Application.Features.Residents.AddResident;
using Lease.Application.Features.Residents.GetResidents;
using Lease.Application.Features.Residents.LinkResidentUser;
using Lease.Application.Features.Residents.RemoveResident;
using Lease.Application.Features.Residents.SetPrimaryResident;
using Lease.Application.Features.ServiceSets.ApplyServiceSet;
using Lease.Application.Features.ServiceSets.GetServiceSetById;
using Lease.Application.Features.ServiceSets.GetServiceSets;
using Lease.Application.Features.ServiceSets.UpsertServiceSet;
using Lease.Application.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Messaging.Contracts.Messages;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Lease.Api.Controllers;

[ApiController]
[Route("api/lease")]
public sealed class LeaseController(IMediatorGateway mediator, IBus bus) : ControllerBase
{
    [HttpPost("leases/from-booking")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> CreateFromBooking([FromBody] CreateLeaseFromBookingRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<LeaseDto>(new CreateFromBookingCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<LeaseDto>.Succeed(dto));
    }

    [HttpPost("leases")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> CreateManual([FromBody] CreateManualLeaseRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<LeaseDto>(new CreateManualLeaseCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<LeaseDto>.Succeed(dto));
    }

    [HttpGet("leases")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaseDto>>>> Search([FromQuery] string? statuses, [FromQuery] Guid? unitId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<LeaseDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return this.ApiBadRequest<IReadOnlyList<LeaseDto>>(pagingError ?? "Invalid paging.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<LeaseDto>>(new SearchLeasesQuery(orgId, statuses, unitId, search, normalizedPage, normalizedPerPage), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LeaseDto>>.Succeed(list));
    }

    [HttpGet("leases/{leaseId:guid}")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> Detail(Guid leaseId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<LeaseDto?>(new GetLeaseByIdQuery(orgId, leaseId), cancellationToken);
        return result is null
            ? this.ApiNotFound<LeaseDto>("Lease not found.")
            : Ok(ApiResponse<LeaseDto>.Succeed(result));
    }

    [HttpGet("tenant/leases")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaseDto>>>> TenantLeases([FromQuery] Guid? userId, CancellationToken cancellationToken)
    {
        var id = userId ?? (HttpContext.TryGetOrgAndUser(out _, out var actor) ? actor : Guid.Empty);
        if (id == Guid.Empty)
        {
            return this.ApiBadRequest<IReadOnlyList<LeaseDto>>("User id is required.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<LeaseDto>>(new GetTenantLeasesQuery(id), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LeaseDto>>.Succeed(list));
    }

    [HttpPut("leases/{leaseId:guid}")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> Update(Guid leaseId, [FromBody] UpdateLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var updated = await mediator.SendAsync<LeaseDto?>(new UpdateLeaseCommand(orgId, userId, new UpdateLeaseRequest(leaseId, body.EndDate, body.CycleId, body.PaymentDay, body.Notes)), cancellationToken);
        return updated is null
            ? this.ApiNotFound<LeaseDto>("Lease not found.")
            : Ok(ApiResponse<LeaseDto>.Succeed(updated));
    }

    [HttpPost("leases/{leaseId:guid}/renew")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> Renew(Guid leaseId, [FromBody] RenewLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<LeaseDto>(new RenewLeaseCommand(orgId, userId, new RenewLeaseRequest(leaseId, body.StartDate, body.EndDate, body.RentAmount, body.DepositAmount, body.CycleId, body.PaymentDay, body.Notes, body.LeaseServiceSetId)), cancellationToken);
        return Ok(ApiResponse<LeaseDto>.Succeed(result));
    }

    [HttpPost("leases/{leaseId:guid}/terminate")]
    public async Task<ActionResult<ApiResponse<LeaseDto>>> Terminate(Guid leaseId, [FromBody] TerminateLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<LeaseDto?>(new TerminateLeaseCommand(orgId, userId, new TerminateLeaseRequest(leaseId, body.TerminationDate, body.Reason, body.CreateRefund, body.RefundAmount, body.RefundNotes, body.Notes)), cancellationToken);
        return result is null
            ? this.ApiNotFound<LeaseDto>("Lease not found.")
            : Ok(ApiResponse<LeaseDto>.Succeed(result));
    }

    [HttpGet("leases/{leaseId:guid}/residents")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaseResidentDto>>>> Residents(Guid leaseId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<LeaseResidentDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<LeaseResidentDto>>(new GetResidentsQuery(orgId, leaseId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LeaseResidentDto>>.Succeed(list));
    }

    [HttpPost("leases/{leaseId:guid}/residents")]
    public async Task<ActionResult<ApiResponse<LeaseResidentDto>>> AddResident(Guid leaseId, [FromBody] AddResidentBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseResidentDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<LeaseResidentDto>(new AddResidentCommand(orgId, userId, new AddResidentRequest(leaseId, body.UserId, body.FullName, body.Phone, body.Email, body.IdNumber, body.Relationship)), cancellationToken);
        return Ok(ApiResponse<LeaseResidentDto>.Succeed(dto));
    }

    [HttpDelete("leases/{leaseId:guid}/residents/{residentId:guid}")]
    public async Task<ActionResult<ApiResponse<LeaseRemoveResidentResponse>>> RemoveResident(Guid leaseId, Guid residentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseRemoveResidentResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await mediator.SendAsync<bool>(new RemoveResidentCommand(orgId, userId, leaseId, residentId), cancellationToken)
            ? Ok(ApiResponse<LeaseRemoveResidentResponse>.Succeed(new LeaseRemoveResidentResponse()))
            : this.ApiNotFound<LeaseRemoveResidentResponse>("Resident not found.");
    }

    [HttpPost("leases/{leaseId:guid}/residents/{residentId:guid}/set-primary")]
    public async Task<ActionResult<ApiResponse<LeaseSetPrimaryResidentResponse>>> SetPrimary(Guid leaseId, Guid residentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseSetPrimaryResidentResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await mediator.SendAsync<bool>(new SetPrimaryResidentCommand(orgId, userId, leaseId, residentId), cancellationToken)
            ? Ok(ApiResponse<LeaseSetPrimaryResidentResponse>.Succeed(new LeaseSetPrimaryResidentResponse()))
            : this.ApiNotFound<LeaseSetPrimaryResidentResponse>("Resident not found.");
    }

    [HttpPost("leases/{leaseId:guid}/residents/{residentId:guid}/link-user")]
    public async Task<ActionResult<ApiResponse<LeaseResidentDto>>> LinkUser(Guid leaseId, Guid residentId, [FromBody] LinkUserBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseResidentDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<LeaseResidentDto?>(new LinkResidentUserCommand(orgId, userId, new LinkResidentUserRequest(leaseId, residentId, body.UserId)), cancellationToken);
        return result is null
            ? this.ApiNotFound<LeaseResidentDto>("Resident not found.")
            : Ok(ApiResponse<LeaseResidentDto>.Succeed(result));
    }

    [HttpPost("leases/{leaseId:guid}/apply-service-set")]
    public async Task<ActionResult<ApiResponse<LeaseApplyServiceSetResponse>>> ApplyServiceSet(Guid leaseId, [FromBody] ApplyServiceSetBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseApplyServiceSetResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await mediator.SendAsync<bool>(new ApplyServiceSetCommand(orgId, userId, new ApplyServiceSetRequest(leaseId, body.LeaseServiceSetId)), cancellationToken)
            ? Ok(ApiResponse<LeaseApplyServiceSetResponse>.Succeed(new LeaseApplyServiceSetResponse()))
            : this.ApiNotFound<LeaseApplyServiceSetResponse>("Lease or service set not found.");
    }

    [HttpGet("lease-service-sets")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaseServiceSetDto>>>> GetServiceSets(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<LeaseServiceSetDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<LeaseServiceSetDto>>(new GetServiceSetsQuery(orgId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LeaseServiceSetDto>>.Succeed(list));
    }

    [HttpGet("lease-service-sets/{id:guid}")]
    public async Task<ActionResult<ApiResponse<LeaseServiceSetDto>>> GetServiceSetById(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<LeaseServiceSetDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<LeaseServiceSetDto?>(new GetServiceSetByIdQuery(orgId, id), cancellationToken);
        return result is null
            ? this.ApiNotFound<LeaseServiceSetDto>("Service set not found.")
            : Ok(ApiResponse<LeaseServiceSetDto>.Succeed(result));
    }

    [HttpPost("lease-service-sets")]
    public async Task<ActionResult<ApiResponse<LeaseServiceSetDto>>> UpsertServiceSet([FromBody] UpsertLeaseServiceSetRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseServiceSetDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<LeaseServiceSetDto>(new UpsertServiceSetCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<LeaseServiceSetDto>.Succeed(dto));
    }

    [HttpGet("payment-cycles")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentCycleDto>>>> GetPaymentCycles(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<PaymentCycleDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<PaymentCycleDto>>(new GetPaymentCyclesQuery(orgId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PaymentCycleDto>>.Succeed(list));
    }

    [HttpPost("payment-cycles")]
    public async Task<ActionResult<ApiResponse<PaymentCycleDto>>> UpsertPaymentCycle([FromBody] UpsertPaymentCycleRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PaymentCycleDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<PaymentCycleDto>(new UpsertPaymentCycleCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<PaymentCycleDto>.Succeed(dto));
    }

    [HttpDelete("payment-cycles/{id:guid}")]
    public async Task<ActionResult<ApiResponse<LeaseDeletePaymentCycleResponse>>> DeletePaymentCycle(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<LeaseDeletePaymentCycleResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await mediator.SendAsync<bool>(new DeletePaymentCycleCommand(orgId, userId, id), cancellationToken)
            ? Ok(ApiResponse<LeaseDeletePaymentCycleResponse>.Succeed(new LeaseDeletePaymentCycleResponse()))
            : this.ApiNotFound<LeaseDeletePaymentCycleResponse>("Payment cycle not found.");
    }

    [HttpGet("master-leases")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MasterLeaseDto>>>> GetMasterLeases(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<MasterLeaseDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var list = await mediator.SendAsync<IReadOnlyList<MasterLeaseDto>>(new GetMasterLeasesQuery(orgId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MasterLeaseDto>>.Succeed(list));
    }

    [HttpPost("master-leases")]
    public async Task<ActionResult<ApiResponse<MasterLeaseDto>>> UpsertMasterLease([FromBody] UpsertMasterLeaseRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<MasterLeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await mediator.SendAsync<MasterLeaseDto>(new UpsertMasterLeaseCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<MasterLeaseDto>.Succeed(dto));
    }

    [HttpPost("master-leases/{id:guid}/terminate")]
    public async Task<ActionResult<ApiResponse<MasterLeaseDto>>> TerminateMasterLease(Guid id, [FromBody] TerminateMasterLeaseBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<MasterLeaseDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await mediator.SendAsync<MasterLeaseDto?>(new TerminateMasterLeaseCommand(orgId, userId, id, body.TerminationDate, body.Reason), cancellationToken);
        return result is null
            ? this.ApiNotFound<MasterLeaseDto>("Master lease not found.")
            : Ok(ApiResponse<MasterLeaseDto>.Succeed(result));
    }

    [HttpPost("internal/jobs/expiring-check")]
    public async Task<ActionResult<ApiResponse<LeaseInternalJobQueuedResponse>>> RunExpiringCheck([FromQuery] DateOnly? asOfDate, CancellationToken cancellationToken)
    {
        var runDate = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        await bus.Publish(new LeaseExpiringCheckRequested { AsOfDate = runDate, CorrelationId = Guid.NewGuid() }, cancellationToken);
        return Accepted(ApiResponse<LeaseInternalJobQueuedResponse>.Succeed(new LeaseInternalJobQueuedResponse(true, runDate)));
    }

    [HttpPost("internal/jobs/expiry-sweep")]
    public async Task<ActionResult<ApiResponse<LeaseInternalJobQueuedResponse>>> RunExpirySweep([FromQuery] DateOnly? asOfDate, CancellationToken cancellationToken)
    {
        var runDate = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        await bus.Publish(new LeaseExpirySweepRequested { AsOfDate = runDate, CorrelationId = Guid.NewGuid() }, cancellationToken);
        return Accepted(ApiResponse<LeaseInternalJobQueuedResponse>.Succeed(new LeaseInternalJobQueuedResponse(true, runDate)));
    }

    private static bool TryNormalizePaging(int page, int perPage, out int normalizedPage, out int normalizedPerPage, out string? error)
    {
        normalizedPage = page;
        normalizedPerPage = perPage;
        error = null;

        if (page < 1) { error = "Page must be greater than or equal to 1."; return false; }
        if (perPage < 1 || perPage > 200) { error = "PerPage must be between 1 and 200."; return false; }

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
