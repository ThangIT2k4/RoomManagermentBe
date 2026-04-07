using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalCommissionPolicyEntity = RoomManagerment.CRM.EntityClasses.CommissionPolicyEntity;
using DomainCommissionPolicyEntity = CRM.Domain.Entities.CommissionPolicyEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class CommissionPolicyRepository(DataAccessAdapter adapter) : ICommissionPolicyRepository
{
    public async Task<DomainCommissionPolicyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.CommissionPolicy.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainCommissionPolicyEntity> AddAsync(DomainCommissionPolicyEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalCommissionPolicyEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            Title = entity.Title,
            CalcType = entity.CalcType,
            TriggerEvent = entity.TriggerEvent,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }

    public async Task<DomainCommissionPolicyEntity> UpdateAsync(DomainCommissionPolicyEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.CommissionPolicy.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainCommissionPolicyEntity), entity.Id);
        }

        dal.Title = entity.Title;
        dal.CalcType = entity.CalcType;
        dal.TriggerEvent = entity.TriggerEvent;
        dal.IsActive = entity.IsActive;
        dal.UpdatedAt = entity.UpdatedAt;
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }
}
