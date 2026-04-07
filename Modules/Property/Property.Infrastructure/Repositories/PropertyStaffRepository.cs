using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class PropertyStaffRepository(DataAccessAdapter adapter) : IPropertyStaffRepository
{
    public async Task<PropertiesUserEntity?> GetAsync(Guid propertyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.PropertiesUser.FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.UserId == userId && x.DeletedAt == null, cancellationToken);
    }

    public async Task<PropertiesUserEntity> AddAsync(PropertiesUserEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }

    public async Task<PropertiesUserEntity> UpdateAsync(PropertiesUserEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }
}
