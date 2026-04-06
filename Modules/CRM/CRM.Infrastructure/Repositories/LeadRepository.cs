using CRM.Application.Services;
using CRM.Domain.Entities;
using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace CRM.Infrastructure.Repositories;

public sealed class LeadRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher) : ILeadRepository
{
    public async Task<LeadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Lead.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<LeadEntity> AddAsync(LeadEntity lead, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.CRM.EntityClasses.LeadEntity
        {
            Id = lead.Id,
            OrganizationId = lead.OrganizationId,
            FullName = lead.FullName,
            Status = lead.Status,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(lead, cancellationToken);
        return lead;
    }

    public async Task<LeadEntity> UpdateAsync(LeadEntity lead, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Lead.Where(x => x.Id == lead.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(LeadEntity), lead.Id);
        }

        dal.FullName = lead.FullName;
        dal.Status = lead.Status;
        dal.UpdatedAt = lead.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(lead, cancellationToken);
        return lead;
    }

    private async Task PublishDomainEventsAsync(LeadEntity lead, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in lead.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        lead.ClearDomainEvents();
    }
}

