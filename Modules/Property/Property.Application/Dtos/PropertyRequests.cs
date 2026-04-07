namespace Property.Application.Dtos;

public sealed record CreatePropertyRequest(
    Guid PropertyTypeId,
    string Name,
    string Address,
    string? Code,
    string? Description,
    string? ProvinceCode,
    string? DistrictCode,
    string? WardCode,
    string? StreetCode,
    decimal? Latitude,
    decimal? Longitude,
    int? NumberOfFloors,
    string? Notes);

public sealed record UpdatePropertyRequest(
    Guid PropertyId,
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

public sealed record CreateUnitRequest(
    Guid PropertyId,
    string Code,
    string? Name,
    int? Floor,
    decimal? AreaM2,
    string? UnitType,
    decimal BaseRent,
    decimal? DepositAmount,
    int? MaxOccupancy,
    string? Note,
    IReadOnlyCollection<Guid>? AmenityIds);

public sealed record UpdateUnitRequest(
    Guid UnitId,
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

public sealed record CreateTicketRequest(
    Guid PropertyId,
    Guid? UnitId,
    Guid? LeaseId,
    string Title,
    string Description,
    Guid? PriorityId);

public sealed record TicketStatusRequest(Guid TicketId, string Status, string? Note, Guid? AssignedTo);

public sealed record CreateMeterRequest(
    Guid PropertyId,
    Guid? UnitId,
    string MeterType,
    string MeterNumber,
    string? Description);

public sealed record AddMeterReadingRequest(
    Guid MeterId,
    DateOnly ReadingDate,
    decimal Value,
    string? ImageUrl,
    string? Note);

public sealed record UpsertAmenityRequest(Guid? Id, string KeyCode, string Name, string? Category, string? Icon);
public sealed record UpsertPropertyTypeRequest(Guid? Id, string KeyCode, string Name, string? Description);
public sealed record AssignPropertyStaffRequest(Guid PropertyId, Guid UserId, string RoleKey);
public sealed record UpsertVendorRequest(
    Guid? Id,
    string Name,
    string? TaxCode,
    string? Phone,
    string? Email,
    string? Address,
    string? VendorType,
    string? ContactPerson,
    string? ContactPhone,
    string? ContactEmail,
    string? BusinessLicense,
    Guid? SepayBankId,
    string? AccountNumber,
    string? AccountHolderName,
    string? BranchName,
    short Status);
public sealed record UploadDocumentRequest(
    string OwnerType,
    Guid OwnerId,
    string FileUrl,
    string FileName,
    string MimeType,
    long FileSize,
    string? DocumentType,
    string? Description,
    bool IsPrimary,
    int SortOrder);
