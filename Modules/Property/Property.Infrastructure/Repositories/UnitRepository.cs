using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class UnitRepository(DataAccessAdapter adapter) : IUnitRepository
{
    public async Task<UnitEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.Unit.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UnitEntity> AddAsync(UnitEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }

    public async Task<UnitEntity> UpdateAsync(UnitEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }
}
