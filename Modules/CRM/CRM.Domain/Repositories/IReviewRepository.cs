using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface IReviewRepository
{
    Task<ReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReviewEntity> AddAsync(ReviewEntity entity, CancellationToken cancellationToken = default);
    Task<ReviewEntity> UpdateAsync(ReviewEntity entity, CancellationToken cancellationToken = default);
}
