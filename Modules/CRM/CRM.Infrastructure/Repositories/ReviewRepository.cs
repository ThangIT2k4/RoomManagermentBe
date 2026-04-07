using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalReviewEntity = RoomManagerment.CRM.EntityClasses.ReviewEntity;
using DomainReviewEntity = CRM.Domain.Entities.ReviewEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class ReviewRepository(DataAccessAdapter adapter) : IReviewRepository
{
    public async Task<DomainReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Review.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainReviewEntity> AddAsync(DomainReviewEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalReviewEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            UnitId = entity.UnitId,
            UserId = entity.UserId,
            Rating = entity.Rating,
            Content = entity.Content,
            IsPublic = entity.IsPublic,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }

    public async Task<DomainReviewEntity> UpdateAsync(DomainReviewEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Review.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainReviewEntity), entity.Id);
        }

        dal.Rating = entity.Rating;
        dal.Content = entity.Content;
        dal.IsPublic = entity.IsPublic;
        dal.UpdatedAt = entity.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }
}
