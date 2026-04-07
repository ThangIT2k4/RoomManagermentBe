using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IUnitRepository
{
    Task<UnitEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnitEntity> AddAsync(UnitEntity entity, CancellationToken cancellationToken = default);
    Task<UnitEntity> UpdateAsync(UnitEntity entity, CancellationToken cancellationToken = default);
}
