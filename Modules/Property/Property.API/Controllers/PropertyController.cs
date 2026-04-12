using Microsoft.AspNetCore.Mvc;
using Property.API;
using Property.Application.Dtos;
using Property.Application.Features.Amenities.DeleteAmenity;
using Property.Application.Features.Amenities.GetAmenities;
using Property.Application.Features.Amenities.UpsertAmenity;
using Property.Application.Features.Dashboard.GetDashboardSummary;
using Property.Application.Features.Documents.DeleteDocument;
using Property.Application.Features.Documents.GetDocuments;
using Property.Application.Features.Documents.UploadDocument;
using Property.Application.Features.Meters.AddMeterReading;
using Property.Application.Features.Meters.CreateMeter;
using Property.Application.Features.Properties.CreateProperty;
using Property.Application.Features.Properties.DeleteProperty;
using Property.Application.Features.Properties.UpdateProperty;
using Property.Application.Features.PropertyTypes.DeletePropertyType;
using Property.Application.Features.PropertyTypes.GetPropertyTypes;
using Property.Application.Features.PropertyTypes.UpsertPropertyType;
using Property.Application.Features.Staff.AssignStaff;
using Property.Application.Features.Staff.UnassignStaff;
using Property.Application.Features.Tickets.ChangeTicketStatus;
using Property.Application.Features.Tickets.CreateTicket;
using Property.Application.Features.Units.CreateUnit;
using Property.Application.Features.Units.DeleteUnit;
using Property.Application.Features.Units.SearchUnits;
using Property.Application.Features.Units.SetUnitStatus;
using Property.Application.Features.Units.UpdateUnit;
using Property.Application.Features.Vendors.DeleteVendor;
using Property.Application.Features.Vendors.SearchVendors;
using Property.Application.Features.Vendors.UpsertVendor;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;

namespace Property.Api.Controllers;

[ApiController]
[Route("api/property")]
public sealed class PropertyController(IAppSender sender) : ControllerBase
{
    [HttpPost("properties")]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> CreateProperty([FromBody] CreatePropertyRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var dto = await sender.Send(new CreatePropertyCommand(orgId, userId, request), cancellationToken);
        return Ok(ApiResponse<PropertyDto>.Succeed(dto));
    }

    [HttpPut("properties/{propertyId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> UpdateProperty(Guid propertyId, [FromBody] UpdatePropertyBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await sender.Send(new UpdatePropertyCommand(orgId, userId, new UpdatePropertyRequest(
            propertyId, body.PropertyTypeId, body.Name, body.Address, body.Description,
            body.ProvinceCode, body.DistrictCode, body.WardCode, body.StreetCode,
            body.Latitude, body.Longitude, body.NumberOfFloors, body.Status, body.Notes)), cancellationToken);

        return result is null
            ? this.ApiNotFound<PropertyDto>("Property not found.")
            : Ok(ApiResponse<PropertyDto>.Succeed(result));
    }

    [HttpDelete("properties/{propertyId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeletePropertyResponse>>> DeleteProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDeletePropertyResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeletePropertyCommand(orgId, userId, propertyId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeletePropertyResponse>.Succeed(new PropertyDeletePropertyResponse()))
            : this.ApiNotFound<PropertyDeletePropertyResponse>("Property not found.");
    }

    [HttpPost("units")]
    public async Task<ActionResult<ApiResponse<UnitDto>>> CreateUnit([FromBody] CreateUnitRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<UnitDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<UnitDto>.Succeed(await sender.Send(new CreateUnitCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpPut("units/{unitId:guid}")]
    public async Task<ActionResult<ApiResponse<UnitDto>>> UpdateUnit(Guid unitId, [FromBody] UpdateUnitBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<UnitDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await sender.Send(new UpdateUnitCommand(orgId, userId, new UpdateUnitRequest(
            unitId, body.Code, body.Name, body.Floor, body.AreaM2, body.UnitType,
            body.BaseRent, body.DepositAmount, body.MaxOccupancy, body.Note, body.Status, body.AmenityIds)), cancellationToken);

        return result is null
            ? this.ApiNotFound<UnitDto>("Unit not found.")
            : Ok(ApiResponse<UnitDto>.Succeed(result));
    }

    [HttpDelete("units/{unitId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeleteUnitResponse>>> DeleteUnit(Guid unitId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDeleteUnitResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeleteUnitCommand(orgId, userId, unitId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeleteUnitResponse>.Succeed(new PropertyDeleteUnitResponse()))
            : this.ApiNotFound<PropertyDeleteUnitResponse>("Unit not found.");
    }

    [HttpPost("units/{unitId:guid}/status")]
    public async Task<ActionResult<ApiResponse<PropertyChangeUnitStatusResponse>>> ChangeUnitStatus(Guid unitId, [FromBody] ChangeUnitStatusBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyChangeUnitStatusResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new SetUnitStatusCommand(orgId, userId, unitId, body.Status, body.Reason), cancellationToken)
            ? Ok(ApiResponse<PropertyChangeUnitStatusResponse>.Succeed(new PropertyChangeUnitStatusResponse()))
            : this.ApiNotFound<PropertyChangeUnitStatusResponse>("Unit not found.");
    }

    [HttpGet("units")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UnitDto>>>> SearchUnits(
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
            return this.ApiBadRequest<IReadOnlyList<UnitDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return this.ApiBadRequest<IReadOnlyList<UnitDto>>(pagingError ?? "Invalid paging.");
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
                    return this.ApiBadRequest<IReadOnlyList<UnitDto>>($"Invalid unit status value '{token}'.");
                }
                parsed.Add(status);
            }
            statusList = parsed;
        }

        var list = await sender.Send(new SearchUnitsQuery(orgId, propertyId, statusList, unitType, minRent, maxRent, search, normalizedPage, normalizedPerPage), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UnitDto>>.Succeed(list));
    }

    [HttpPost("tickets")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<TicketDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<TicketDto>.Succeed(await sender.Send(new CreateTicketCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpPost("tickets/{ticketId:guid}/status")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> ChangeTicketStatus(Guid ticketId, [FromBody] ChangeTicketStatusBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<TicketDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        var result = await sender.Send(new ChangeTicketStatusCommand(orgId, userId, new TicketStatusRequest(ticketId, body.Status, body.Note, body.AssignedTo)), cancellationToken);
        return result is null
            ? this.ApiNotFound<TicketDto>("Ticket not found.")
            : Ok(ApiResponse<TicketDto>.Succeed(result));
    }

    [HttpPost("meters")]
    public async Task<ActionResult<ApiResponse<MeterDto>>> CreateMeter([FromBody] CreateMeterRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<MeterDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<MeterDto>.Succeed(await sender.Send(new CreateMeterCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpPost("meters/{meterId:guid}/readings")]
    public async Task<ActionResult<ApiResponse<PropertyAddMeterReadingResponse>>> AddMeterReading(Guid meterId, [FromBody] AddMeterReadingBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyAddMeterReadingResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new AddMeterReadingCommand(orgId, userId, new AddMeterReadingRequest(meterId, body.ReadingDate, body.Value, body.ImageUrl, body.Note)), cancellationToken)
            ? Ok(ApiResponse<PropertyAddMeterReadingResponse>.Succeed(new PropertyAddMeterReadingResponse()))
            : this.ApiNotFound<PropertyAddMeterReadingResponse>("Meter not found.");
    }

    [HttpGet("dashboard/property-summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> Dashboard(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<DashboardSummaryDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<DashboardSummaryDto>.Succeed(await sender.Send(new GetDashboardSummaryQuery(orgId), cancellationToken)));
    }

    [HttpGet("amenities")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AmenityDto>>>> GetAmenities([FromQuery] string? category, CancellationToken cancellationToken)
    {
        var list = await sender.Send(new GetAmenitiesQuery(category), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AmenityDto>>.Succeed(list));
    }

    [HttpPost("amenities")]
    public async Task<ActionResult<ApiResponse<AmenityDto>>> UpsertAmenity([FromBody] UpsertAmenityRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<AmenityDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<AmenityDto>.Succeed(await sender.Send(new UpsertAmenityCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpDelete("amenities/{amenityId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeleteAmenityResponse>>> DeleteAmenity(Guid amenityId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out _, out var userId))
        {
            return this.ApiBadRequest<PropertyDeleteAmenityResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeleteAmenityCommand(userId, amenityId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeleteAmenityResponse>.Succeed(new PropertyDeleteAmenityResponse()))
            : this.ApiNotFound<PropertyDeleteAmenityResponse>("Amenity not found.");
    }

    [HttpGet("property-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PropertyTypeDto>>>> GetPropertyTypes(CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<PropertyTypeDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<IReadOnlyList<PropertyTypeDto>>.Succeed(await sender.Send(new GetPropertyTypesQuery(orgId), cancellationToken)));
    }

    [HttpPost("property-types")]
    public async Task<ActionResult<ApiResponse<PropertyTypeDto>>> UpsertPropertyType([FromBody] UpsertPropertyTypeRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyTypeDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<PropertyTypeDto>.Succeed(await sender.Send(new UpsertPropertyTypeCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpDelete("property-types/{propertyTypeId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeletePropertyTypeResponse>>> DeletePropertyType(Guid propertyTypeId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDeletePropertyTypeResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeletePropertyTypeCommand(orgId, userId, propertyTypeId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeletePropertyTypeResponse>.Succeed(new PropertyDeletePropertyTypeResponse()))
            : this.ApiNotFound<PropertyDeletePropertyTypeResponse>("Property type not found.");
    }

    [HttpPost("properties/{propertyId:guid}/staff")]
    public async Task<ActionResult<ApiResponse<PropertyStaffDto>>> AssignStaff(Guid propertyId, [FromBody] AssignStaffBody body, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyStaffDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<PropertyStaffDto>.Succeed(await sender.Send(new AssignStaffCommand(orgId, userId, new AssignPropertyStaffRequest(propertyId, body.UserId, body.RoleKey)), cancellationToken)));
    }

    [HttpDelete("properties/{propertyId:guid}/staff/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyUnassignStaffResponse>>> UnassignStaff(Guid propertyId, Guid userId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var actorId))
        {
            return this.ApiBadRequest<PropertyUnassignStaffResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new UnassignStaffCommand(orgId, actorId, propertyId, userId), cancellationToken)
            ? Ok(ApiResponse<PropertyUnassignStaffResponse>.Succeed(new PropertyUnassignStaffResponse()))
            : this.ApiNotFound<PropertyUnassignStaffResponse>("Staff assignment not found.");
    }

    [HttpGet("vendors")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VendorDto>>>> SearchVendors([FromQuery] string? vendorType, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<VendorDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        if (!TryNormalizePaging(page, perPage, out var normalizedPage, out var normalizedPerPage, out var pagingError))
        {
            return this.ApiBadRequest<IReadOnlyList<VendorDto>>(pagingError ?? "Invalid paging.");
        }

        var list = await sender.Send(new SearchVendorsQuery(orgId, vendorType, search, normalizedPage, normalizedPerPage), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<VendorDto>>.Succeed(list));
    }

    [HttpPost("vendors")]
    public async Task<ActionResult<ApiResponse<VendorDto>>> UpsertVendor([FromBody] UpsertVendorRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<VendorDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<VendorDto>.Succeed(await sender.Send(new UpsertVendorCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpDelete("vendors/{vendorId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeleteVendorResponse>>> DeleteVendor(Guid vendorId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDeleteVendorResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeleteVendorCommand(orgId, userId, vendorId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeleteVendorResponse>.Succeed(new PropertyDeleteVendorResponse()))
            : this.ApiNotFound<PropertyDeleteVendorResponse>("Vendor not found.");
    }

    [HttpPost("documents")]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> UploadDocument([FromBody] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<DocumentDto>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<DocumentDto>.Succeed(await sender.Send(new UploadDocumentCommand(orgId, userId, request), cancellationToken)));
    }

    [HttpGet("documents")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DocumentDto>>>> GetDocuments([FromQuery] string ownerType, [FromQuery] Guid ownerId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out _))
        {
            return this.ApiBadRequest<IReadOnlyList<DocumentDto>>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return Ok(ApiResponse<IReadOnlyList<DocumentDto>>.Succeed(await sender.Send(new GetDocumentsQuery(orgId, ownerType, ownerId), cancellationToken)));
    }

    [HttpDelete("documents/{documentId:guid}")]
    public async Task<ActionResult<ApiResponse<PropertyDeleteDocumentResponse>>> DeleteDocument(Guid documentId, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetOrgAndUser(out var orgId, out var userId))
        {
            return this.ApiBadRequest<PropertyDeleteDocumentResponse>("Headers X-Organization-Id and X-User-Id are required.");
        }

        return await sender.Send(new DeleteDocumentCommand(orgId, userId, documentId), cancellationToken)
            ? Ok(ApiResponse<PropertyDeleteDocumentResponse>.Succeed(new PropertyDeleteDocumentResponse()))
            : this.ApiNotFound<PropertyDeleteDocumentResponse>("Document not found.");
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
