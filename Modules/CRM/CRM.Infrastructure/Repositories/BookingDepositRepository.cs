using CRM.Application.Services;
using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalBookingDepositEntity = RoomManagerment.CRM.EntityClasses.BookingDepositEntity;
using DomainBookingDepositEntity = CRM.Domain.Entities.BookingDepositEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class BookingDepositRepository(DataAccessAdapter adapter, IIntegrationEventPublisher eventPublisher) : IBookingDepositRepository
{
    public async Task<DomainBookingDepositEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.BookingDeposit.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainBookingDepositEntity> AddAsync(DomainBookingDepositEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalBookingDepositEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            LeadId = entity.LeadId,
            ViewingId = entity.ViewingId,
            Amount = entity.Amount,
            DepositType = entity.DepositType,
            PaymentStatus = entity.PaymentStatus,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<DomainBookingDepositEntity> UpdateAsync(DomainBookingDepositEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.BookingDeposit.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainBookingDepositEntity), entity.Id);
        }

        dal.Amount = entity.Amount;
        dal.DepositType = entity.DepositType;
        dal.PaymentStatus = entity.PaymentStatus;
        dal.UpdatedAt = entity.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        await PublishDomainEventsAsync(entity, cancellationToken);
        return entity;
    }

    private async Task PublishDomainEventsAsync(DomainBookingDepositEntity entity, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        entity.ClearDomainEvents();
    }
}
