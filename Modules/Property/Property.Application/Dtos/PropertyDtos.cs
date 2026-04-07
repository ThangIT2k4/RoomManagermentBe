namespace Property.Application.Dtos;

public sealed record PropertyDto(
    Guid Id,
    Guid OrganizationId,
    Guid PropertyTypeId,
    string Name,
    string? Code,
    string Address,
    short Status,
    int TotalUnits);

public sealed record UnitDto(
    Guid Id,
    Guid PropertyId,
    string Code,
    string? Name,
    short Status,
    decimal BaseRent,
    decimal? DepositAmount,
    int? Floor,
    decimal? AreaM2,
    string? UnitType);

public sealed record TicketDto(
    Guid Id,
    Guid PropertyId,
    Guid? UnitId,
    string Title,
    string Description,
    string Status,
    Guid CreatedBy,
    Guid? AssignedTo,
    Guid? PriorityId,
    DateTime CreatedAt);

public sealed record MeterDto(
    Guid Id,
    Guid PropertyId,
    Guid? UnitId,
    string MeterType,
    string MeterNumber,
    bool IsActive);

public sealed record DashboardSummaryDto(
    int AvailableUnits,
    int RentedUnits,
    int MaintenanceUnits,
    int TotalUnits,
    decimal OccupancyRate,
    int OpenTickets,
    int InProgressTickets);

public sealed record AmenityDto(Guid Id, string KeyCode, string Name, string? Category, string? Icon);
public sealed record PropertyTypeDto(Guid Id, Guid? OrganizationId, string KeyCode, string Name, string? Description);
public sealed record PropertyStaffDto(Guid Id, Guid PropertyId, Guid UserId, string RoleKey, DateTime AssignedAt);
public sealed record VendorDto(Guid Id, Guid OrganizationId, string Name, string? VendorType, string? Phone, string? Email, short Status);
public sealed record DocumentDto(
    Guid Id,
    Guid OrganizationId,
    string OwnerType,
    Guid OwnerId,
    string FileUrl,
    string FileName,
    string MimeType,
    long FileSize,
    string? DocumentType,
    bool IsPrimary,
    int SortOrder);
