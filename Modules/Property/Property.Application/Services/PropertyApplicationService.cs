using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Application;

public sealed class PropertyApplicationService(IDataAccessAdapterFactory adapterFactory) : IPropertyApplicationService
{
    public async Task<PropertyDto> CreatePropertyAsync(Guid organizationId, Guid userId, CreatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Property name is required.");
        if (string.IsNullOrWhiteSpace(request.Address)) throw new ArgumentException("Property address is required.");

        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var entity = new PropertyEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            PropertyTypeId = request.PropertyTypeId,
            Code = request.Code?.Trim(),
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            ProvinceCode = request.ProvinceCode?.Trim(),
            DistrictCode = request.DistrictCode?.Trim(),
            WardCode = request.WardCode?.Trim(),
            StreetCode = request.StreetCode?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            NumberOfFloors = request.NumberOfFloors,
            Notes = request.Notes?.Trim(),
            TotalUnits = 0,
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(Guid organizationId, Guid userId, UpdatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Property.FirstOrDefaultAsync(x => x.Id == request.PropertyId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return null;

        entity.PropertyTypeId = request.PropertyTypeId;
        entity.Name = request.Name.Trim();
        entity.Address = request.Address.Trim();
        entity.ProvinceCode = request.ProvinceCode?.Trim();
        entity.DistrictCode = request.DistrictCode?.Trim();
        entity.WardCode = request.WardCode?.Trim();
        entity.StreetCode = request.StreetCode?.Trim();
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.NumberOfFloors = request.NumberOfFloors;
        entity.Notes = request.Notes?.Trim();
        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeletePropertyAsync(Guid organizationId, Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Property.FirstOrDefaultAsync(x => x.Id == propertyId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<UnitDto> CreateUnitAsync(Guid organizationId, Guid userId, CreateUnitRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();

        var entity = new UnitEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            PropertyId = request.PropertyId,
            Code = request.Code.Trim(),
            Name = request.Name?.Trim(),
            Floor = request.Floor,
            AreaM2 = request.AreaM2,
            UnitType = request.UnitType?.Trim(),
            BaseRent = request.BaseRent,
            DepositAmount = request.DepositAmount,
            MaxOccupancy = request.MaxOccupancy,
            Note = request.Note?.Trim(),
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<UnitDto?> UpdateUnitAsync(Guid organizationId, Guid userId, UpdateUnitRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Unit.FirstOrDefaultAsync(x => x.Id == request.UnitId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return null;

        entity.Code = request.Code.Trim();
        entity.Name = request.Name?.Trim();
        entity.Floor = request.Floor;
        entity.AreaM2 = request.AreaM2;
        entity.UnitType = request.UnitType?.Trim();
        entity.BaseRent = request.BaseRent;
        entity.DepositAmount = request.DepositAmount;
        entity.MaxOccupancy = request.MaxOccupancy;
        entity.Note = request.Note?.Trim();
        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteUnitAsync(Guid organizationId, Guid userId, Guid unitId, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Unit.FirstOrDefaultAsync(x => x.Id == unitId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<bool> SetUnitStatusAsync(Guid organizationId, Guid userId, Guid unitId, short status, string? reason, CancellationToken cancellationToken = default)
    {
        _ = reason;
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Unit.FirstOrDefaultAsync(x => x.Id == unitId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<UnitDto>> SearchUnitsAsync(Guid organizationId, Guid? propertyId, IReadOnlyCollection<short>? statuses, string? unitType, decimal? minRent, decimal? maxRent, string? search, int page, int perPage, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var query = linq.Unit.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null);

        if (propertyId.HasValue) query = query.Where(x => x.PropertyId == propertyId.Value);
        if (statuses is { Count: > 0 }) query = query.Where(x => statuses.Contains(x.Status));
        if (!string.IsNullOrWhiteSpace(unitType)) query = query.Where(x => x.UnitType == unitType);
        if (minRent.HasValue) query = query.Where(x => x.BaseRent >= minRent.Value);
        if (maxRent.HasValue) query = query.Where(x => x.BaseRent <= maxRent.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Code.ToLower().Contains(keyword) || x.Name.ToLower().Contains(keyword));
        }

        var skip = Math.Max(page - 1, 0) * Math.Max(perPage, 1);
        var entities = await query.OrderBy(x => x.PropertyId).ThenBy(x => x.Floor).ThenBy(x => x.Code).Skip(skip).Take(perPage).ToListAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    public async Task<TicketDto> CreateTicketAsync(Guid organizationId, Guid userId, CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var entity = new TicketEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            PropertyId = request.PropertyId,
            UnitId = request.UnitId,
            LeaseId = request.LeaseId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            PriorityId = request.PriorityId,
            Status = "open",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<TicketDto?> ChangeTicketStatusAsync(Guid organizationId, Guid userId, TicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        _ = userId;
        _ = request.Note;
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Ticket.FirstOrDefaultAsync(x => x.Id == request.TicketId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return null;
        entity.Status = request.Status.Trim().ToLowerInvariant();
        entity.AssignedTo = request.AssignedTo;
        if (entity.Status == "cancelled")
        {
            entity.CancelledAt = DateTime.UtcNow;
            entity.CancelledBy = userId;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<MeterDto> CreateMeterAsync(Guid organizationId, Guid userId, CreateMeterRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var entity = new MeterEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            PropertyId = request.PropertyId,
            UnitId = request.UnitId,
            MeterType = request.MeterType.Trim(),
            MeterNumber = request.MeterNumber.Trim(),
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Map(entity);
    }

    public async Task<bool> AddMeterReadingAsync(Guid organizationId, Guid userId, AddMeterReadingRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var meterExists = await linq.Meter.AnyAsync(x => x.Id == request.MeterId && x.OrganizationId == organizationId && x.DeletedAt == null && x.IsActive, cancellationToken);
        if (!meterExists) return false;

        var lastValue = await linq.MeterReading
            .Where(x => x.MeterId == request.MeterId && x.DeletedAt == null)
            .OrderByDescending(x => x.ReadingDate)
            .Select(x => (decimal?)x.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastValue.HasValue && request.Value < lastValue.Value) throw new InvalidOperationException("New reading must be greater than or equal to latest reading.");

        var existed = await linq.MeterReading.AnyAsync(x => x.MeterId == request.MeterId && x.ReadingDate == request.ReadingDate && x.DeletedAt == null, cancellationToken);
        if (existed) throw new InvalidOperationException("Reading already exists for the same date.");

        var entity = new MeterReadingEntity
        {
            Id = Guid.NewGuid(),
            MeterId = request.MeterId,
            ReadingDate = request.ReadingDate,
            Value = request.Value,
            ImageUrl = request.ImageUrl?.Trim(),
            Note = request.Note?.Trim(),
            TakenBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);

        var availableUnits = await linq.Unit.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null && x.Status == 1, cancellationToken);
        var rentedUnits = await linq.Unit.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null && x.Status == 2, cancellationToken);
        var maintenanceUnits = await linq.Unit.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null && x.Status == 3, cancellationToken);
        var totalUnits = await linq.Unit.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        var openTickets = await linq.Ticket.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null && x.Status == "open", cancellationToken);
        var inProgressTickets = await linq.Ticket.CountAsync(x => x.OrganizationId == organizationId && x.DeletedAt == null && x.Status == "in_progress", cancellationToken);

        var occupancyRate = totalUnits == 0 ? 0m : Math.Round(rentedUnits * 100m / totalUnits, 1);
        return new DashboardSummaryDto(availableUnits, rentedUnits, maintenanceUnits, totalUnits, occupancyRate, openTickets, inProgressTickets);
    }

    public async Task<AmenityDto> UpsertAmenityAsync(Guid organizationId, Guid userId, UpsertAmenityRequest request, CancellationToken cancellationToken = default)
    {
        _ = organizationId;
        _ = userId;
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        AmenityEntity entity;
        if (request.Id.HasValue)
        {
            var linq = new LinqMetaData(adapter);
            entity = await linq.Amenity.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.DeletedAt == null, cancellationToken)
                ?? throw new InvalidOperationException("Amenity not found.");
            entity.KeyCode = request.KeyCode.Trim();
            entity.Name = request.Name.Trim();
            entity.Category = request.Category?.Trim();
            entity.Icon = request.Icon?.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;
        }
        else
        {
            entity = new AmenityEntity
            {
                Id = Guid.NewGuid(),
                KeyCode = request.KeyCode.Trim(),
                Name = request.Name.Trim(),
                Category = request.Category?.Trim(),
                Icon = request.Icon?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
        }

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return new AmenityDto(entity.Id, entity.KeyCode, entity.Name, entity.Category, entity.Icon);
    }

    public async Task<IReadOnlyList<AmenityDto>> GetAmenitiesAsync(string? category, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var query = linq.Amenity.Where(x => x.DeletedAt == null);
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category == category);
        }

        var rows = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
        return rows.Select(x => new AmenityDto(x.Id, x.KeyCode, x.Name, x.Category, x.Icon)).ToList();
    }

    public async Task<bool> DeleteAmenityAsync(Guid userId, Guid amenityId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Amenity.FirstOrDefaultAsync(x => x.Id == amenityId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<PropertyTypeDto> UpsertPropertyTypeAsync(Guid organizationId, Guid userId, UpsertPropertyTypeRequest request, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        PropertyTypeEntity entity;
        if (request.Id.HasValue)
        {
            var linq = new LinqMetaData(adapter);
            entity = await linq.PropertyType.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.DeletedAt == null, cancellationToken)
                ?? throw new InvalidOperationException("Property type not found.");
            entity.KeyCode = request.KeyCode.Trim();
            entity.Name = request.Name.Trim();
            entity.Description = request.Description?.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;
        }
        else
        {
            entity = new PropertyTypeEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                KeyCode = request.KeyCode.Trim(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
        }

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return new PropertyTypeDto(entity.Id, entity.OrganizationId, entity.KeyCode, entity.Name, entity.Description);
    }

    public async Task<IReadOnlyList<PropertyTypeDto>> GetPropertyTypesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.PropertyType
            .Where(x => x.DeletedAt == null && (x.OrganizationId == null || x.OrganizationId == organizationId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return rows.Select(x => new PropertyTypeDto(x.Id, x.OrganizationId, x.KeyCode, x.Name, x.Description)).ToList();
    }

    public async Task<bool> DeletePropertyTypeAsync(Guid organizationId, Guid userId, Guid propertyTypeId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.PropertyType.FirstOrDefaultAsync(x => x.Id == propertyTypeId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<PropertyStaffDto> AssignStaffAsync(Guid organizationId, Guid userId, AssignPropertyStaffRequest request, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var entity = new PropertiesUserEntity
        {
            Id = Guid.NewGuid(),
            PropertyId = request.PropertyId,
            UserId = request.UserId,
            RoleKey = request.RoleKey.Trim(),
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _ = organizationId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return new PropertyStaffDto(entity.Id, entity.PropertyId, entity.UserId, entity.RoleKey, entity.AssignedAt);
    }

    public async Task<bool> UnassignStaffAsync(Guid organizationId, Guid userId, Guid propertyId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        _ = organizationId;
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.PropertiesUser.FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.UserId == targetUserId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<VendorDto> UpsertVendorAsync(Guid organizationId, Guid userId, UpsertVendorRequest request, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        VendorEntity entity;
        if (request.Id.HasValue)
        {
            var linq = new LinqMetaData(adapter);
            entity = await linq.Vendor.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken)
                ?? throw new InvalidOperationException("Vendor not found.");
            entity.Name = request.Name.Trim();
            entity.TaxCode = request.TaxCode?.Trim();
            entity.Phone = request.Phone?.Trim();
            entity.Email = request.Email?.Trim();
            entity.Address = request.Address?.Trim();
            entity.VendorType = request.VendorType?.Trim();
            entity.ContactPerson = request.ContactPerson?.Trim();
            entity.ContactPhone = request.ContactPhone?.Trim();
            entity.ContactEmail = request.ContactEmail?.Trim();
            entity.BusinessLicense = request.BusinessLicense?.Trim();
            entity.SepayBankId = request.SepayBankId;
            entity.AccountNumber = request.AccountNumber?.Trim();
            entity.AccountHolderName = request.AccountHolderName?.Trim();
            entity.BranchName = request.BranchName?.Trim();
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;
        }
        else
        {
            entity = new VendorEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Name = request.Name.Trim(),
                TaxCode = request.TaxCode?.Trim(),
                Phone = request.Phone?.Trim(),
                Email = request.Email?.Trim(),
                Address = request.Address?.Trim(),
                VendorType = request.VendorType?.Trim(),
                ContactPerson = request.ContactPerson?.Trim(),
                ContactPhone = request.ContactPhone?.Trim(),
                ContactEmail = request.ContactEmail?.Trim(),
                BusinessLicense = request.BusinessLicense?.Trim(),
                SepayBankId = request.SepayBankId,
                AccountNumber = request.AccountNumber?.Trim(),
                AccountHolderName = request.AccountHolderName?.Trim(),
                BranchName = request.BranchName?.Trim(),
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
        }

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return new VendorDto(entity.Id, entity.OrganizationId, entity.Name, entity.VendorType, entity.Phone, entity.Email, entity.Status);
    }

    public async Task<IReadOnlyList<VendorDto>> SearchVendorsAsync(Guid organizationId, string? vendorType, string? search, int page, int perPage, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var query = linq.Vendor.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null);
        if (!string.IsNullOrWhiteSpace(vendorType)) query = query.Where(x => x.VendorType == vendorType);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Phone.ToLower().Contains(keyword));
        }

        var skip = Math.Max(page - 1, 0) * Math.Max(perPage, 1);
        var rows = await query.OrderBy(x => x.Name).Skip(skip).Take(perPage).ToListAsync(cancellationToken);
        return rows.Select(x => new VendorDto(x.Id, x.OrganizationId, x.Name, x.VendorType, x.Phone, x.Email, x.Status)).ToList();
    }

    public async Task<bool> DeleteVendorAsync(Guid organizationId, Guid userId, Guid vendorId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Vendor.FirstOrDefaultAsync(x => x.Id == vendorId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    public async Task<DocumentDto> UploadDocumentAsync(Guid organizationId, Guid userId, UploadDocumentRequest request, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var entity = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            OwnerType = request.OwnerType.Trim(),
            OwnerId = request.OwnerId,
            FileUrl = request.FileUrl.Trim(),
            FileName = request.FileName.Trim(),
            MimeType = request.MimeType.Trim(),
            FileSize = request.FileSize,
            DocumentType = request.DocumentType?.Trim(),
            Description = request.Description?.Trim(),
            IsPrimary = request.IsPrimary,
            SortOrder = request.SortOrder,
            UploadedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return new DocumentDto(entity.Id, entity.OrganizationId, entity.OwnerType, entity.OwnerId, entity.FileUrl, entity.FileName, entity.MimeType, entity.FileSize, entity.DocumentType, entity.IsPrimary, entity.SortOrder);
    }

    public async Task<IReadOnlyList<DocumentDto>> GetDocumentsAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.Document
            .Where(x => x.OrganizationId == organizationId && x.OwnerType == ownerType && x.OwnerId == ownerId && x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(x => new DocumentDto(x.Id, x.OrganizationId, x.OwnerType, x.OwnerId, x.FileUrl, x.FileName, x.MimeType, x.FileSize, x.DocumentType, x.IsPrimary, x.SortOrder)).ToList();
    }

    public async Task<bool> DeleteDocumentAsync(Guid organizationId, Guid userId, Guid documentId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var entity = await linq.Document.FirstOrDefaultAsync(x => x.Id == documentId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return true;
    }

    private static void ValidateOrgAndUser(Guid organizationId, Guid userId)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization id is required.");
        if (userId == Guid.Empty) throw new ArgumentException("User id is required.");
    }

    private static PropertyDto Map(PropertyEntity entity) =>
        new(entity.Id, entity.OrganizationId, entity.PropertyTypeId, entity.Name, entity.Code, entity.Address, entity.Status, entity.TotalUnits);

    private static UnitDto Map(UnitEntity entity) =>
        new(entity.Id, entity.PropertyId, entity.Code, entity.Name, entity.Status, entity.BaseRent, entity.DepositAmount, entity.Floor, entity.AreaM2, entity.UnitType);

    private static TicketDto Map(TicketEntity entity) =>
        new(entity.Id, entity.PropertyId, entity.UnitId, entity.Title, entity.Description, entity.Status, entity.CreatedBy, entity.AssignedTo, entity.PriorityId, entity.CreatedAt);

    private static MeterDto Map(MeterEntity entity) =>
        new(entity.Id, entity.PropertyId, entity.UnitId, entity.MeterType, entity.MeterNumber, entity.IsActive);
}
