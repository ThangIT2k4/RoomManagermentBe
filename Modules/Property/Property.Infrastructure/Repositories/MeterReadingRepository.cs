using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class MeterReadingRepository(DataAccessAdapter adapter) : IMeterReadingRepository
{
    public async Task<MeterReadingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.MeterReading.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MeterReadingEntity?> GetLatestAsync(Guid meterId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.MeterReading
            .Where(x => x.MeterId == meterId && x.DeletedAt == null)
            .OrderByDescending(x => x.ReadingDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MeterReadingEntity> AddAsync(MeterReadingEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }
}
