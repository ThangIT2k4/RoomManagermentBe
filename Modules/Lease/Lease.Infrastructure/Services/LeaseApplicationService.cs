using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Lease.DatabaseSpecific;
using RoomManagerment.Lease.EntityClasses;
using RoomManagerment.Lease.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Lease.Infrastructure.Services;

public sealed class LeaseApplicationService(IDataAccessAdapterFactory adapterFactory) : ILeaseApplicationService
{
    public async Task<LeaseDto> CreateFromBookingAsync(Guid organizationId, Guid userId, CreateLeaseFromBookingRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        ValidateLeaseDates(request.StartDate, request.EndDate);

        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var lease = new LeaseEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UnitId = request.UnitId,
            LeaseNo = await NextLeaseNoAsync(adapter, organizationId, cancellationToken),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RentAmount = request.RentAmount,
            DepositAmount = request.DepositAmount,
            CycleId = request.CycleId,
            PaymentDay = request.PaymentDay,
            Status = "active",
            BookingId = request.BookingId,
            Notes = AppendServiceSetToNotes(request.Notes, request.LeaseServiceSetId),
            CreatedAt = DateTime.UtcNow
        };

        var resident = new LeaseResidentEntity
        {
            Id = Guid.NewGuid(),
            LeaseId = lease.Id,
            UserId = request.TenantUserId,
            FullName = "Primary Tenant",
            Relationship = "primary",
            IsPrimary = true,
            CreatedAt = DateTime.UtcNow
        };

        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        await adapter.SaveEntityAsync(resident, true, false, cancellationToken);
        return MapLease(lease);
    }

    public async Task<LeaseDto> CreateManualAsync(Guid organizationId, Guid userId, CreateManualLeaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        ValidateLeaseDates(request.StartDate, request.EndDate);

        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var lease = new LeaseEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UnitId = request.UnitId,
            LeaseNo = await NextLeaseNoAsync(adapter, organizationId, cancellationToken),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RentAmount = request.RentAmount,
            DepositAmount = request.DepositAmount,
            CycleId = request.CycleId,
            PaymentDay = request.PaymentDay,
            Status = "active",
            Notes = AppendServiceSetToNotes(request.Notes, request.LeaseServiceSetId),
            CreatedAt = DateTime.UtcNow
        };

        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        await adapter.SaveEntityAsync(new LeaseResidentEntity
        {
            Id = Guid.NewGuid(),
            LeaseId = lease.Id,
            UserId = request.TenantUserId,
            FullName = request.TenantFullName?.Trim() ?? "Primary Tenant",
            Phone = request.TenantPhone?.Trim(),
            Email = request.TenantEmail?.Trim(),
            IdNumber = request.TenantIdNumber?.Trim(),
            Relationship = "primary",
            IsPrimary = true,
            CreatedAt = DateTime.UtcNow
        }, true, false, cancellationToken);

        return MapLease(lease);
    }

    public async Task<IReadOnlyList<LeaseDto>> SearchLeasesAsync(Guid organizationId, string? statuses, Guid? unitId, string? search, int page, int perPage, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var query = linq.Lease.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null);

        if (unitId.HasValue)
        {
            query = query.Where(x => x.UnitId == unitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(statuses))
        {
            var normalized = statuses.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLowerInvariant()).ToArray();
            query = query.Where(x => normalized.Contains(x.Status.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.LeaseNo.ToLower().Contains(keyword));
        }

        var skip = Math.Max(page - 1, 0) * Math.Max(perPage, 1);
        var rows = await query.OrderByDescending(x => x.StartDate).Skip(skip).Take(perPage).ToListAsync(cancellationToken);
        return rows.Select(MapLease).ToList();
    }

    public async Task<LeaseDto?> GetLeaseByIdAsync(Guid organizationId, Guid leaseId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var lease = await linq.Lease.FirstOrDefaultAsync(x => x.Id == leaseId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        return lease is null ? null : MapLease(lease);
    }

    public async Task<IReadOnlyList<LeaseDto>> GetTenantLeasesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var leases = await (from l in linq.Lease
            join r in linq.LeaseResident on l.Id equals r.LeaseId
            where r.UserId == userId && l.DeletedAt == null && r.DeletedAt == null
            select l).OrderByDescending(x => x.StartDate).ToListAsync(cancellationToken);
        return leases.Select(MapLease).ToList();
    }

    public async Task<LeaseDto?> UpdateLeaseAsync(Guid organizationId, Guid userId, UpdateLeaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var lease = await linq.Lease.FirstOrDefaultAsync(x => x.Id == request.LeaseId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (lease is null || lease.Status != "active")
        {
            return null;
        }

        lease.EndDate = request.EndDate;
        lease.CycleId = request.CycleId;
        lease.PaymentDay = request.PaymentDay;
        lease.Notes = request.Notes?.Trim();
        lease.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        return MapLease(lease);
    }

    public async Task<LeaseDto> RenewLeaseAsync(Guid organizationId, Guid userId, RenewLeaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var oldLease = await linq.Lease.FirstOrDefaultAsync(x => x.Id == request.OldLeaseId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken)
                       ?? throw new InvalidOperationException("Old lease not found.");

        var newLease = new LeaseEntity
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UnitId = oldLease.UnitId,
            ParentLeaseId = oldLease.Id,
            LeaseNo = await NextLeaseNoAsync(adapter, organizationId, cancellationToken),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RentAmount = request.RentAmount,
            DepositAmount = request.DepositAmount,
            CycleId = request.CycleId,
            PaymentDay = request.PaymentDay,
            Status = "active",
            Notes = AppendServiceSetToNotes(request.Notes, request.LeaseServiceSetId),
            CreatedAt = DateTime.UtcNow
        };

        await adapter.SaveEntityAsync(newLease, true, false, cancellationToken);

        var residents = await linq.LeaseResident.Where(x => x.LeaseId == oldLease.Id && x.DeletedAt == null).ToListAsync(cancellationToken);
        foreach (var resident in residents)
        {
            await adapter.SaveEntityAsync(new LeaseResidentEntity
            {
                Id = Guid.NewGuid(),
                LeaseId = newLease.Id,
                UserId = resident.UserId,
                FullName = resident.FullName,
                Phone = resident.Phone,
                Email = resident.Email,
                IdNumber = resident.IdNumber,
                Relationship = resident.Relationship,
                IsPrimary = resident.IsPrimary,
                CreatedAt = DateTime.UtcNow
            }, true, false, cancellationToken);
        }

        oldLease.Status = "renewed";
        oldLease.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(oldLease, true, false, cancellationToken);
        return MapLease(newLease);
    }

    public async Task<LeaseDto?> TerminateLeaseAsync(Guid organizationId, Guid userId, TerminateLeaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var lease = await linq.Lease.FirstOrDefaultAsync(x => x.Id == request.LeaseId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (lease is null || lease.Status != "active")
        {
            return null;
        }

        lease.Status = "terminated";
        lease.EndDate = request.TerminationDate;
        lease.Notes = string.IsNullOrWhiteSpace(request.Notes) ? lease.Notes : request.Notes.Trim();
        lease.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        return MapLease(lease);
    }

    public async Task<LeaseResidentDto> AddResidentAsync(Guid organizationId, Guid userId, AddResidentRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var resident = new LeaseResidentEntity
        {
            Id = Guid.NewGuid(),
            LeaseId = request.LeaseId,
            UserId = request.UserId,
            FullName = request.FullName.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            IdNumber = request.IdNumber?.Trim(),
            Relationship = request.Relationship?.Trim(),
            IsPrimary = false,
            CreatedAt = DateTime.UtcNow
        };
        await adapter.SaveEntityAsync(resident, true, false, cancellationToken);
        return MapResident(resident);
    }

    public async Task<bool> RemoveResidentAsync(Guid organizationId, Guid userId, Guid leaseId, Guid residentId, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var resident = await linq.LeaseResident.FirstOrDefaultAsync(x => x.Id == residentId && x.LeaseId == leaseId && x.DeletedAt == null, cancellationToken);
        if (resident is null)
        {
            return false;
        }

        resident.DeletedAt = DateTime.UtcNow;
        resident.DeletedBy = userId;
        resident.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(resident, true, false, cancellationToken);
        return true;
    }

    public async Task<bool> SetPrimaryResidentAsync(Guid organizationId, Guid userId, Guid leaseId, Guid residentId, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var residents = await linq.LeaseResident.Where(x => x.LeaseId == leaseId && x.DeletedAt == null).ToListAsync(cancellationToken);
        if (residents.Count == 0)
        {
            return false;
        }

        foreach (var resident in residents)
        {
            resident.IsPrimary = resident.Id == residentId;
            resident.UpdatedAt = DateTime.UtcNow;
            await adapter.SaveEntityAsync(resident, true, false, cancellationToken);
        }

        return true;
    }

    public async Task<LeaseResidentDto?> LinkResidentUserAsync(Guid organizationId, Guid userId, LinkResidentUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var resident = await linq.LeaseResident.FirstOrDefaultAsync(x => x.LeaseId == request.LeaseId && x.Id == request.ResidentId && x.DeletedAt == null, cancellationToken);
        if (resident is null)
        {
            return null;
        }

        resident.UserId = request.UserId;
        resident.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(resident, true, false, cancellationToken);
        return MapResident(resident);
    }

    public async Task<IReadOnlyList<LeaseResidentDto>> GetResidentsAsync(Guid organizationId, Guid leaseId, CancellationToken cancellationToken = default)
    {
        _ = organizationId;
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.LeaseResident.Where(x => x.LeaseId == leaseId && x.DeletedAt == null).OrderByDescending(x => x.IsPrimary).ThenBy(x => x.FullName).ToListAsync(cancellationToken);
        return rows.Select(MapResident).ToList();
    }

    public async Task<bool> ApplyServiceSetAsync(Guid organizationId, Guid userId, ApplyServiceSetRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var lease = await linq.Lease.FirstOrDefaultAsync(x => x.Id == request.LeaseId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (lease is null || lease.Status != "active")
        {
            return false;
        }

        lease.Notes = AppendServiceSetToNotes(lease.Notes, request.LeaseServiceSetId);
        lease.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        return true;
    }

    public async Task<LeaseServiceSetDto> UpsertServiceSetAsync(Guid organizationId, Guid userId, UpsertLeaseServiceSetRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        LeaseServiceSetEntity set;
        if (request.Id.HasValue)
        {
            set = await linq.LeaseServiceSet.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken)
                  ?? throw new InvalidOperationException("Service set not found.");
            set.Name = request.Name.Trim();
            set.Description = request.Description?.Trim();
            set.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            set = new LeaseServiceSetEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        await adapter.SaveEntityAsync(set, true, false, cancellationToken);

        var existingItems = await linq.LeaseServiceSetItem.Where(x => x.LeaseServiceSetId == set.Id && x.DeletedAt == null).ToListAsync(cancellationToken);
        foreach (var item in existingItems)
        {
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = userId;
            await adapter.SaveEntityAsync(item, true, false, cancellationToken);
        }

        foreach (var item in request.Items)
        {
            await adapter.SaveEntityAsync(new LeaseServiceSetItemEntity
            {
                Id = Guid.NewGuid(),
                LeaseServiceSetId = set.Id,
                ServiceId = item.ServiceId,
                Price = item.Price,
                IsRequired = item.IsRequired,
                CreatedAt = DateTime.UtcNow
            }, true, false, cancellationToken);
        }

        return (await GetServiceSetByIdAsync(organizationId, set.Id, cancellationToken))!;
    }

    public async Task<IReadOnlyList<LeaseServiceSetDto>> GetServiceSetsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var sets = await linq.LeaseServiceSet.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null).OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var result = new List<LeaseServiceSetDto>(sets.Count);
        foreach (var set in sets)
        {
            var items = await linq.LeaseServiceSetItem.Where(x => x.LeaseServiceSetId == set.Id && x.DeletedAt == null).ToListAsync(cancellationToken);
            result.Add(new LeaseServiceSetDto(set.Id, set.OrganizationId, set.Name, set.Description, items.Select(MapServiceSetItem).ToList()));
        }

        return result;
    }

    public async Task<LeaseServiceSetDto?> GetServiceSetByIdAsync(Guid organizationId, Guid serviceSetId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var set = await linq.LeaseServiceSet.FirstOrDefaultAsync(x => x.Id == serviceSetId && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (set is null)
        {
            return null;
        }

        var items = await linq.LeaseServiceSetItem.Where(x => x.LeaseServiceSetId == set.Id && x.DeletedAt == null).ToListAsync(cancellationToken);
        return new LeaseServiceSetDto(set.Id, set.OrganizationId, set.Name, set.Description, items.Select(MapServiceSetItem).ToList());
    }

    public async Task<PaymentCycleDto> UpsertPaymentCycleAsync(Guid organizationId, Guid userId, UpsertPaymentCycleRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        PaymentCycleEntity cycle;
        if (request.Id.HasValue)
        {
            cycle = await linq.PaymentCycle.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken)
                    ?? throw new InvalidOperationException("Payment cycle not found.");
            cycle.Name = request.Name.Trim();
            cycle.DurationMonths = request.DurationMonths;
            cycle.DayOfMonth = request.DayOfMonth;
            cycle.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            cycle = new PaymentCycleEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Name = request.Name.Trim(),
                DurationMonths = request.DurationMonths,
                DayOfMonth = request.DayOfMonth,
                CreatedAt = DateTime.UtcNow
            };
        }

        await adapter.SaveEntityAsync(cycle, true, false, cancellationToken);
        return MapCycle(cycle);
    }

    public async Task<IReadOnlyList<PaymentCycleDto>> GetPaymentCyclesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.PaymentCycle.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null).OrderBy(x => x.Name).ToListAsync(cancellationToken);
        return rows.Select(MapCycle).ToList();
    }

    public async Task<bool> DeletePaymentCycleAsync(Guid organizationId, Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var cycle = await linq.PaymentCycle.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (cycle is null)
        {
            return false;
        }

        cycle.DeletedAt = DateTime.UtcNow;
        cycle.DeletedBy = userId;
        cycle.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(cycle, true, false, cancellationToken);
        return true;
    }

    public async Task<MasterLeaseDto> UpsertMasterLeaseAsync(Guid organizationId, Guid userId, UpsertMasterLeaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        MasterLeaseEntity entity;
        if (request.Id.HasValue)
        {
            entity = await linq.MasterLease.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken)
                     ?? throw new InvalidOperationException("Master lease not found.");
            entity.LandlordUserId = request.LandlordUserId;
            entity.PropertyId = request.PropertyId;
            entity.ContractNo = request.ContractNo?.Trim();
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.RentAmount = request.RentAmount;
            entity.DepositAmount = request.DepositAmount;
            entity.PaymentDay = request.PaymentDay;
            entity.Notes = request.Notes?.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new MasterLeaseEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                LandlordUserId = request.LandlordUserId,
                PropertyId = request.PropertyId,
                ContractNo = request.ContractNo?.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RentAmount = request.RentAmount,
                DepositAmount = request.DepositAmount,
                PaymentDay = request.PaymentDay,
                Status = "active",
                Notes = request.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return MapMasterLease(entity);
    }

    public async Task<MasterLeaseDto?> TerminateMasterLeaseAsync(Guid organizationId, Guid userId, Guid id, DateOnly terminationDate, string reason, CancellationToken cancellationToken = default)
    {
        _ = reason;
        ValidateOrgAndUser(organizationId, userId);
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var lease = await linq.MasterLease.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId && x.DeletedAt == null, cancellationToken);
        if (lease is null)
        {
            return null;
        }

        lease.EndDate = terminationDate;
        lease.Status = "terminated";
        lease.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        return MapMasterLease(lease);
    }

    public async Task<IReadOnlyList<MasterLeaseDto>> GetMasterLeasesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.MasterLease.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null).OrderByDescending(x => x.StartDate).ToListAsync(cancellationToken);
        return rows.Select(MapMasterLease).ToList();
    }

    public async Task<int> RunExpiringLeaseCheckAsync(DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var targetDates = new[]
        {
            asOfDate.AddDays(30),
            asOfDate.AddDays(14),
            asOfDate.AddDays(7),
            asOfDate.AddDays(3),
            asOfDate.AddDays(1)
        };

        var rows = await linq.Lease.Where(x => x.Status == "active" && x.DeletedAt == null && targetDates.Contains(x.EndDate)).ToListAsync(cancellationToken);
        return rows.Count;
    }

    public async Task<int> RunLeaseExpirySweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.Lease.Where(x => x.Status == "active" && x.DeletedAt == null && x.EndDate < asOfDate).ToListAsync(cancellationToken);
        foreach (var lease in rows)
        {
            lease.Status = "expired";
            lease.UpdatedAt = DateTime.UtcNow;
            await adapter.SaveEntityAsync(lease, true, false, cancellationToken);
        }

        return rows.Count;
    }

    private static void ValidateLeaseDates(DateOnly startDate, DateOnly endDate)
    {
        if (endDate <= startDate)
        {
            throw new InvalidOperationException("End date must be greater than start date.");
        }
    }

    private static void ValidateOrgAndUser(Guid organizationId, Guid userId)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization id is required.");
        if (userId == Guid.Empty) throw new ArgumentException("User id is required.");
    }

    private static string? AppendServiceSetToNotes(string? notes, Guid? serviceSetId)
    {
        if (!serviceSetId.HasValue)
        {
            return notes?.Trim();
        }

        return $"{notes?.Trim()} [service_set:{serviceSetId}]".Trim();
    }

    private static LeaseDto MapLease(LeaseEntity lease) =>
        new(lease.Id, lease.UnitId, lease.OrganizationId, lease.LeaseNo, lease.Status, lease.StartDate, lease.EndDate, lease.RentAmount, lease.DepositAmount, lease.CycleId, lease.PaymentDay, lease.ParentLeaseId, lease.BookingId, lease.Notes);

    private static LeaseResidentDto MapResident(LeaseResidentEntity resident) =>
        new(resident.Id, resident.LeaseId, resident.UserId, resident.FullName, resident.Phone, resident.Email, resident.IdNumber, resident.Relationship, resident.IsPrimary);

    private static LeaseServiceSetItemDto MapServiceSetItem(LeaseServiceSetItemEntity item) => new(item.Id, item.ServiceId, item.Price, item.IsRequired);
    private static PaymentCycleDto MapCycle(PaymentCycleEntity item) => new(item.Id, item.OrganizationId, item.Name, item.DurationMonths, item.DayOfMonth);
    private static MasterLeaseDto MapMasterLease(MasterLeaseEntity item) => new(item.Id, item.OrganizationId, item.PropertyId, item.LandlordUserId, item.ContractNo, item.StartDate, item.EndDate, item.RentAmount, item.DepositAmount, item.PaymentDay, item.Status, item.Notes);

    private static async Task<string> NextLeaseNoAsync(DataAccessAdapter adapter, Guid organizationId, CancellationToken cancellationToken)
    {
        var linq = new LinqMetaData(adapter);
        var year = DateTime.UtcNow.Year;
        var prefix = $"HD{year}-";
        var count = await linq.Lease.CountAsync(x => x.OrganizationId == organizationId && x.LeaseNo.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):0000}";
    }
}
