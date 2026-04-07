using Microsoft.AspNetCore.Mvc;
using Property.API;
using Property.Application.Dtos;
using Property.Application.Services;

namespace Property.Api.Controllers;

[ApiController]
[Route("api/property")]
public sealed class PropertyController(IPropertyApplicationService service) : ControllerBase
{
    [HttpPost("properties")]
    public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] CreatePropertyRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.CreatePropertyAsync(orgId, userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("properties/{propertyId:guid}")]
    public async Task<ActionResult<PropertyDto>> UpdateProperty(Guid propertyId, [FromBody] UpdatePropertyBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.UpdatePropertyAsync(orgId, userId, new UpdatePropertyRequest(
            propertyId,
            body.PropertyTypeId,
            body.Name,
            body.Address,
            body.Description,
            body.ProvinceCode,
            body.DistrictCode,
            body.WardCode,
            body.StreetCode,
            body.Latitude,
            body.Longitude,
            body.NumberOfFloors,
            body.Status,
            body.Notes), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("properties/{propertyId:guid}")]
    public async Task<IActionResult> DeleteProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var deleted = await service.DeletePropertyAsync(orgId, userId, propertyId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("units")]
    public async Task<ActionResult<UnitDto>> CreateUnit([FromBody] CreateUnitRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(await service.CreateUnitAsync(orgId, userId, request, cancellationToken));
    }

    [HttpPut("units/{unitId:guid}")]
    public async Task<ActionResult<UnitDto>> UpdateUnit(Guid unitId, [FromBody] UpdateUnitBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.UpdateUnitAsync(orgId, userId, new UpdateUnitRequest(
            unitId,
            body.Code,
            body.Name,
            body.Floor,
            body.AreaM2,
            body.UnitType,
            body.BaseRent,
            body.DepositAmount,
            body.MaxOccupancy,
            body.Note,
            body.Status,
            body.AmenityIds), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("units/{unitId:guid}")]
    public async Task<IActionResult> DeleteUnit(Guid unitId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var deleted = await service.DeleteUnitAsync(orgId, userId, unitId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("units/{unitId:guid}/status")]
    public async Task<IActionResult> ChangeUnitStatus(Guid unitId, [FromBody] ChangeUnitStatusBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var changed = await service.SetUnitStatusAsync(orgId, userId, unitId, body.Status, body.Reason, cancellationToken);
        return changed ? Ok() : NotFound();
    }

    [HttpGet("units")]
    public async Task<ActionResult<IReadOnlyList<UnitDto>>> SearchUnits(
        [FromQuery] Guid? propertyId,
        [FromQuery] string? statuses,
        [FromQuery] string? unitType,
        [FromQuery] decimal? minRent,
        [FromQuery] decimal? maxRent,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return BadRequest(new { error = pagingError });
        }

        IReadOnlyCollection<short>? statusList = null;
        if (!string.IsNullOrWhiteSpace(statuses))
        {
            var tokens = statuses.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var parsed = new List<short>(tokens.Length);
            foreach (var token in tokens)
            {
                if (!short.TryParse(token, out var status))
                {
                    return BadRequest(new { error = $"Invalid unit status value '{token}'." });
                }

                parsed.Add(status);
            }

            statusList = parsed;
        }

        var result = await service.SearchUnitsAsync(orgId, propertyId, statusList, unitType, minRent, maxRent, search, normalizedPage, normalizedPerPage, cancellationToken);
        return Ok(result);
    }

    [HttpPost("tickets")]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.CreateTicketAsync(orgId, userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("tickets/{ticketId:guid}/status")]
    public async Task<ActionResult<TicketDto>> ChangeTicketStatus(Guid ticketId, [FromBody] ChangeTicketStatusBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.ChangeTicketStatusAsync(orgId, userId, new TicketStatusRequest(ticketId, body.Status, body.Note, body.AssignedTo), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("meters")]
    public async Task<ActionResult<MeterDto>> CreateMeter([FromBody] CreateMeterRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.CreateMeterAsync(orgId, userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("meters/{meterId:guid}/readings")]
    public async Task<IActionResult> AddMeterReading(Guid meterId, [FromBody] AddMeterReadingBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var ok = await service.AddMeterReadingAsync(orgId, userId, new AddMeterReadingRequest(meterId, body.ReadingDate, body.Value, body.ImageUrl, body.Note), cancellationToken);
        return ok ? Ok() : NotFound();
    }

    [HttpGet("dashboard/property-summary")]
    public async Task<ActionResult<DashboardSummaryDto>> Dashboard(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await service.GetDashboardSummaryAsync(orgId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("amenities")]
    public async Task<ActionResult<IReadOnlyList<AmenityDto>>> GetAmenities([FromQuery] string? category, CancellationToken cancellationToken)
        => Ok(await service.GetAmenitiesAsync(category, cancellationToken));

    [HttpPost("amenities")]
    public async Task<ActionResult<AmenityDto>> UpsertAmenity([FromBody] UpsertAmenityRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertAmenityAsync(orgId, userId, request, cancellationToken));
    }

    [HttpDelete("amenities/{amenityId:guid}")]
    public async Task<IActionResult> DeleteAmenity(Guid amenityId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out _, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.DeleteAmenityAsync(userId, amenityId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("property-types")]
    public async Task<ActionResult<IReadOnlyList<PropertyTypeDto>>> GetPropertyTypes(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetPropertyTypesAsync(orgId, cancellationToken));
    }

    [HttpPost("property-types")]
    public async Task<ActionResult<PropertyTypeDto>> UpsertPropertyType([FromBody] UpsertPropertyTypeRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertPropertyTypeAsync(orgId, userId, request, cancellationToken));
    }

    [HttpDelete("property-types/{propertyTypeId:guid}")]
    public async Task<IActionResult> DeletePropertyType(Guid propertyTypeId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.DeletePropertyTypeAsync(orgId, userId, propertyTypeId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPost("properties/{propertyId:guid}/staff")]
    public async Task<ActionResult<PropertyStaffDto>> AssignStaff(Guid propertyId, [FromBody] AssignStaffBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        var result = await service.AssignStaffAsync(orgId, userId, new AssignPropertyStaffRequest(propertyId, body.UserId, body.RoleKey), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("properties/{propertyId:guid}/staff/{userId:guid}")]
    public async Task<IActionResult> UnassignStaff(Guid propertyId, Guid userId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var actorId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.UnassignStaffAsync(orgId, actorId, propertyId, userId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("vendors")]
    public async Task<ActionResult<IReadOnlyList<VendorDto>>> SearchVendors([FromQuery] string? vendorType, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return BadRequest(new { error = pagingError });
        }

        return Ok(await service.SearchVendorsAsync(orgId, vendorType, search, normalizedPage, normalizedPerPage, cancellationToken));
    }

    [HttpPost("vendors")]
    public async Task<ActionResult<VendorDto>> UpsertVendor([FromBody] UpsertVendorRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UpsertVendorAsync(orgId, userId, request, cancellationToken));
    }

    [HttpDelete("vendors/{vendorId:guid}")]
    public async Task<IActionResult> DeleteVendor(Guid vendorId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.DeleteVendorAsync(orgId, userId, vendorId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPost("documents")]
    public async Task<ActionResult<DocumentDto>> UploadDocument([FromBody] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.UploadDocumentAsync(orgId, userId, request, cancellationToken));
    }

    [HttpGet("documents")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetDocuments([FromQuery] string ownerType, [FromQuery] Guid ownerId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return Ok(await service.GetDocumentsAsync(orgId, ownerType, ownerId, cancellationToken));
    }

    [HttpDelete("documents/{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId)) return BadRequest("Headers X-Organization-Id and X-User-Id are required.");
        return await service.DeleteDocumentAsync(orgId, userId, documentId, cancellationToken) ? NoContent() : NotFound();
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

public sealed record UpdatePropertyBody(
    Guid PropertyTypeId,
    string Name,
    string Address,
    string? Description,
    string? ProvinceCode,
    string? DistrictCode,
    string? WardCode,
    string? StreetCode,
    decimal? Latitude,
    decimal? Longitude,
    int? NumberOfFloors,
    short Status,
    string? Notes);

public sealed record UpdateUnitBody(
    string Code,
    string? Name,
    int? Floor,
    decimal? AreaM2,
    string? UnitType,
    decimal BaseRent,
    decimal? DepositAmount,
    int? MaxOccupancy,
    string? Note,
    short Status,
    IReadOnlyCollection<Guid>? AmenityIds);

public sealed record ChangeUnitStatusBody(short Status, string? Reason);
public sealed record ChangeTicketStatusBody(string Status, string? Note, Guid? AssignedTo);
public sealed record AddMeterReadingBody(DateOnly ReadingDate, decimal Value, string? ImageUrl, string? Note);
public sealed record AssignStaffBody(Guid UserId, string RoleKey);
