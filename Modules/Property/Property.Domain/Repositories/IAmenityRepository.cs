using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IAmenityRepository
{
    Task<AmenityEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AmenityEntity> AddAsync(AmenityEntity entity, CancellationToken cancellationToken = default);
    Task<AmenityEntity> UpdateAsync(AmenityEntity entity, CancellationToken cancellationToken = default);
}
