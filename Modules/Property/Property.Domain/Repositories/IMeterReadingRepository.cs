using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IMeterReadingRepository
{
    Task<MeterReadingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MeterReadingEntity?> GetLatestAsync(Guid meterId, CancellationToken cancellationToken = default);
    Task<MeterReadingEntity> AddAsync(MeterReadingEntity entity, CancellationToken cancellationToken = default);
}
