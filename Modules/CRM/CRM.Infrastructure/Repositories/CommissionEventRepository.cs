using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalCommissionEventEntity = RoomManagerment.CRM.EntityClasses.CommissionEventEntity;
using DomainCommissionEventEntity = CRM.Domain.Entities.CommissionEventEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class CommissionEventRepository(DataAccessAdapter adapter) : ICommissionEventRepository
{
    public async Task<DomainCommissionEventEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.CommissionEvent.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainCommissionEventEntity> AddAsync(DomainCommissionEventEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalCommissionEventEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            PolicyId = entity.PolicyId,
            AgentId = entity.AgentId,
            CommissionTotal = entity.CommissionTotal,
            OccurredAt = entity.OccurredAt,
            Status = entity.Status,
            TriggerEvent = entity.TriggerEvent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }

    public async Task<DomainCommissionEventEntity> UpdateAsync(DomainCommissionEventEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.CommissionEvent.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainCommissionEventEntity), entity.Id);
        }

        dal.Status = entity.Status;
        dal.CommissionTotal = entity.CommissionTotal;
        dal.UpdatedAt = entity.UpdatedAt;
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }
}
