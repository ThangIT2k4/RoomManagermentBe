using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IPropertyTypeRepository
{
    Task<PropertyTypeEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PropertyTypeEntity> AddAsync(PropertyTypeEntity entity, CancellationToken cancellationToken = default);
    Task<PropertyTypeEntity> UpdateAsync(PropertyTypeEntity entity, CancellationToken cancellationToken = default);
}
