using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IMeterRepository
{
    Task<MeterEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MeterEntity> AddAsync(MeterEntity entity, CancellationToken cancellationToken = default);
    Task<MeterEntity> UpdateAsync(MeterEntity entity, CancellationToken cancellationToken = default);
}
