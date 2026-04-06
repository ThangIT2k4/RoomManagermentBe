using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface IReviewReplyRepository
{
    Task<ReviewReplyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReviewReplyEntity> AddAsync(ReviewReplyEntity entity, CancellationToken cancellationToken = default);
    Task<ReviewReplyEntity> UpdateAsync(ReviewReplyEntity entity, CancellationToken cancellationToken = default);
}
