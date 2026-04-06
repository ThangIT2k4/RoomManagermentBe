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

    public async Task<LeaseEntity> AddAsync(LeaseEntity lease, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Lease.EntityClasses.LeaseEntity
        {
            Id = lease.Id,
            UnitId = lease.UnitId,
            OrganizationId = lease.OrganizationId,
            LeaseNo = lease.LeaseNo,
            RentAmount = lease.RentAmount,
            Status = lease.Status,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            CreatedAt = lease.CreatedAt,
            UpdatedAt = lease.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return lease;
    }
}

