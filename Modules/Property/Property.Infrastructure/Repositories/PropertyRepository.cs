using Property.Domain.Entities;
using Property.Domain.Repositories;
using Property.Infrastructure.Mapper;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class PropertyRepository(DataAccessAdapter adapter) : IPropertyRepository
{
    public async Task<PropertyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Property.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<PropertyEntity> AddAsync(PropertyEntity property, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Property.EntityClasses.PropertyEntity
        {
            Id = property.Id,
            OrganizationId = property.OrganizationId,
            Name = property.Name,
            Code = property.Code,
            Status = property.Status,
            TotalUnits = property.TotalUnits,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return property;
    }
}

