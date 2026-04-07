using CRM.Domain.Exceptions;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Mapper;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalReviewReplyEntity = RoomManagerment.CRM.EntityClasses.ReviewReplyEntity;
using DomainReviewReplyEntity = CRM.Domain.Entities.ReviewReplyEntity;

namespace CRM.Infrastructure.Repositories;

public sealed class ReviewReplyRepository(DataAccessAdapter adapter) : IReviewReplyRepository
{
    public async Task<DomainReviewReplyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.ReviewReply.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<DomainReviewReplyEntity> AddAsync(DomainReviewReplyEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new DalReviewReplyEntity
        {
            Id = entity.Id,
            ReviewId = entity.ReviewId,
            UserId = entity.UserId,
            Content = entity.Content,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }

    public async Task<DomainReviewReplyEntity> UpdateAsync(DomainReviewReplyEntity entity, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.ReviewReply.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            throw new EntityNotFoundException(nameof(DomainReviewReplyEntity), entity.Id);
        }

        dal.Content = entity.Content;
        dal.UpdatedAt = entity.UpdatedAt;
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        entity.ClearDomainEvents();
        return entity;
    }
}
