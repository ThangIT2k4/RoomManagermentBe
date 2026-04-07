using CRM.Application.Services;
using CRM.Domain.Entities;
using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalViewingEntity = RoomManagerment.CRM.EntityClasses.ViewingEntity;
using DomainViewingEntity = CRM.Domain.Entities.ViewingEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class ViewingRepository(DataAccessAdapter adapter, IIntegrationEventPublisher eventPublisher) : IViewingRepository
{
    public async Task<DomainViewingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Viewing.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainViewingEntity> AddAsync(DomainViewingEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalViewingEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            LeadId = entity.LeadId,
            AgentId = entity.AgentId,
            ScheduleAt = entity.ScheduleAt,
            Status = entity.Status,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<DomainViewingEntity> UpdateAsync(DomainViewingEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Viewing.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainViewingEntity), entity.Id);
        }

        dal.Status = entity.Status;
        dal.Note = entity.Note;
        dal.UpdatedAt = entity.UpdatedAt;
        dal.ScheduleAt = entity.ScheduleAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(entity, cancellationToken);
        return entity;
    }

    private async Task PublishDomainEventsAsync(DomainViewingEntity entity, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        entity.ClearDomainEvents();
    }
}
