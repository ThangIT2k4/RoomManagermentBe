using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface IViewingRepository
{
    Task<ViewingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ViewingEntity> AddAsync(ViewingEntity entity, CancellationToken cancellationToken = default);
    Task<ViewingEntity> UpdateAsync(ViewingEntity entity, CancellationToken cancellationToken = default);
}
