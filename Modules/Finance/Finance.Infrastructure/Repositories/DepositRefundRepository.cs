using Finance.Domain.Entities;
using Finance.Domain.Repositories;
using Finance.Infrastructure.Mapper;
using RoomManagerment.Finance.DatabaseSpecific;
using RoomManagerment.Finance.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalDepositRefund = RoomManagerment.Finance.EntityClasses.DepositRefundEntity;

namespace Finance.Infrastructure.Repositories;

public sealed class DepositRefundRepository(DataAccessAdapter adapter) : IDepositRefundRepository
{
    public async Task AddAsync(DepositRefundEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = ToDal(entity);
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }

    public async Task UpdateAsync(DepositRefundEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.DepositRefund.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken)
                  ?? throw new InvalidOperationException("Deposit refund not found.");

        dal.Status = entity.Status;
        dal.Notes = entity.Notes;
        dal.UpdatedAt = entity.UpdatedAt;
        dal.PaidAt = entity.PaidAt;
        dal.PaidBy = entity.PaidBy;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }

    public async Task<DepositRefundEntity?> GetByIdAsync(
        Guid id,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.DepositRefund
            .Where(x => x.Id == id && x.OrganizationId == organizationId && x.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<bool> HasPendingOrApprovedForLeaseAsync(Guid leaseId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.DepositRefund.AnyAsync(
            x => x.LeaseId == leaseId
                 && x.DeletedAt == null
                 && (x.Status == "pending" || x.Status == "approved"),
            cancellationToken);
    }

    private static DalDepositRefund ToDal(DepositRefundEntity entity)
    {
        return new DalDepositRefund
        {
            Id = entity.Id,
            LeaseId = entity.LeaseId,
            OrganizationId = entity.OrganizationId,
            TenantId = entity.TenantId,
            Amount = entity.Amount,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            PaidAt = entity.PaidAt,
            PaidBy = entity.PaidBy,
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy
        };
    }
}
