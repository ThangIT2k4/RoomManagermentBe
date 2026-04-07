using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class PropertyTypeRepository(DataAccessAdapter adapter) : IPropertyTypeRepository
{
    public async Task<PropertyTypeEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.PropertyType.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PropertyTypeEntity> AddAsync(PropertyTypeEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }

    public async Task<PropertyTypeEntity> UpdateAsync(PropertyTypeEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }
}
