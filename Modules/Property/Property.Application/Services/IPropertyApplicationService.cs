using Property.Application.Dtos;

namespace Property.Application.Services;

public interface IPropertyApplicationService
{
    Task<PropertyDto> CreatePropertyAsync(Guid organizationId, Guid userId, CreatePropertyRequest request, CancellationToken cancellationToken = default);
    Task<PropertyDto?> UpdatePropertyAsync(Guid organizationId, Guid userId, UpdatePropertyRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePropertyAsync(Guid organizationId, Guid userId, Guid propertyId, CancellationToken cancellationToken = default);

    Task<UnitDto> CreateUnitAsync(Guid organizationId, Guid userId, CreateUnitRequest request, CancellationToken cancellationToken = default);
    Task<UnitDto?> UpdateUnitAsync(Guid organizationId, Guid userId, UpdateUnitRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUnitAsync(Guid organizationId, Guid userId, Guid unitId, CancellationToken cancellationToken = default);
    Task<bool> SetUnitStatusAsync(Guid organizationId, Guid userId, Guid unitId, short status, string? reason, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitDto>> SearchUnitsAsync(Guid organizationId, Guid? propertyId, IReadOnlyCollection<short>? statuses, string? unitType, decimal? minRent, decimal? maxRent, string? search, int page, int perPage, CancellationToken cancellationToken = default);

    Task<TicketDto> CreateTicketAsync(Guid organizationId, Guid userId, CreateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDto?> ChangeTicketStatusAsync(Guid organizationId, Guid userId, TicketStatusRequest request, CancellationToken cancellationToken = default);

    Task<MeterDto> CreateMeterAsync(Guid organizationId, Guid userId, CreateMeterRequest request, CancellationToken cancellationToken = default);
    Task<bool> AddMeterReadingAsync(Guid organizationId, Guid userId, AddMeterReadingRequest request, CancellationToken cancellationToken = default);

    Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<AmenityDto> UpsertAmenityAsync(Guid organizationId, Guid userId, UpsertAmenityRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AmenityDto>> GetAmenitiesAsync(string? category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAmenityAsync(Guid userId, Guid amenityId, CancellationToken cancellationToken = default);

    Task<PropertyTypeDto> UpsertPropertyTypeAsync(Guid organizationId, Guid userId, UpsertPropertyTypeRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyTypeDto>> GetPropertyTypesAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> DeletePropertyTypeAsync(Guid organizationId, Guid userId, Guid propertyTypeId, CancellationToken cancellationToken = default);

    Task<PropertyStaffDto> AssignStaffAsync(Guid organizationId, Guid userId, AssignPropertyStaffRequest request, CancellationToken cancellationToken = default);
    Task<bool> UnassignStaffAsync(Guid organizationId, Guid userId, Guid propertyId, Guid targetUserId, CancellationToken cancellationToken = default);

    Task<VendorDto> UpsertVendorAsync(Guid organizationId, Guid userId, UpsertVendorRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VendorDto>> SearchVendorsAsync(Guid organizationId, string? vendorType, string? search, int page, int perPage, CancellationToken cancellationToken = default);
    Task<bool> DeleteVendorAsync(Guid organizationId, Guid userId, Guid vendorId, CancellationToken cancellationToken = default);

    Task<DocumentDto> UploadDocumentAsync(Guid organizationId, Guid userId, UploadDocumentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentDto>> GetDocumentsAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentAsync(Guid organizationId, Guid userId, Guid documentId, CancellationToken cancellationToken = default);
}
