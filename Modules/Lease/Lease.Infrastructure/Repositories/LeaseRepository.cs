using Lease.Domain.Entities;
using Lease.Domain.Repositories;
using Lease.Infrastructure.Mapper;
using RoomManagerment.Lease.DatabaseSpecific;
using RoomManagerment.Lease.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Lease.Infrastructure.Repositories;

public sealed class LeaseRepository(DataAccessAdapter adapter) : ILeaseRepository
{
    public async Task<LeaseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Lease.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<LeaseEntity?> GetByBookingIdAsync(Guid bookingId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Lease.Where(x => x.BookingId == bookingId && x.OrganizationId == organizationId && x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<IReadOnlyList<LeaseEntity>> GetByUnitIdAsync(Guid unitId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var rows = await linq.Lease.Where(x => x.UnitId == unitId && x.OrganizationId == organizationId && x.DeletedAt == null)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
        return rows.Select(x => x.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<LeaseEntity>> SearchAsync(Guid organizationId, IReadOnlyCollection<string>? statuses, Guid? unitId, string? search, int page, int perPage, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var query = linq.Lease.Where(x => x.OrganizationId == organizationId && x.DeletedAt == null);

        if (unitId.HasValue)
        {
            query = query.Where(x => x.UnitId == unitId.Value);
        }

        if (statuses is { Count: > 0 })
        {
            var normalized = statuses.Select(x => x.Trim().ToLowerInvariant()).ToArray();
            query = query.Where(x => normalized.Contains(x.Status.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.LeaseNo.ToLower().Contains(keyword));
        }

        var skip = Math.Max(page - 1, 0) * Math.Max(perPage, 1);
        var rows = await query.OrderByDescending(x => x.StartDate).Skip(skip).Take(perPage).ToListAsync(cancellationToken);
        return rows.Select(x => x.ToDomain()).ToList();
    }

    public async Task<LeaseEntity> AddAsync(LeaseEntity lease, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Lease.EntityClasses.LeaseEntity
        {
            Id = lease.Id,
            UnitId = lease.UnitId,
            OrganizationId = lease.OrganizationId,
            LeaseNo = lease.LeaseNo,
            RentAmount = lease.RentAmount,
            DepositAmount = lease.DepositAmount,
            CycleId = lease.CycleId,
            PaymentDay = lease.PaymentDay,
            Status = lease.Status,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            ParentLeaseId = lease.ParentLeaseId,
            BookingId = lease.BookingId,
            Notes = lease.Notes,
            CreatedAt = lease.CreatedAt,
            UpdatedAt = lease.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return lease;
    }

    public async Task UpdateAsync(LeaseEntity lease, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Lease.FirstOrDefaultAsync(x => x.Id == lease.Id, cancellationToken);
        if (dal is null)
        {
            return;
        }

        dal.LeaseNo = lease.LeaseNo;
        dal.RentAmount = lease.RentAmount;
        dal.DepositAmount = lease.DepositAmount;
        dal.CycleId = lease.CycleId;
        dal.PaymentDay = lease.PaymentDay;
        dal.Status = lease.Status;
        dal.StartDate = lease.StartDate;
        dal.EndDate = lease.EndDate;
        dal.ParentLeaseId = lease.ParentLeaseId;
        dal.BookingId = lease.BookingId;
        dal.Notes = lease.Notes;
        dal.UpdatedAt = lease.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }
}

