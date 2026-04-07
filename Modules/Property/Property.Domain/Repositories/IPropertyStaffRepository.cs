using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IPropertyStaffRepository
{
    Task<PropertiesUserEntity?> GetAsync(Guid propertyId, Guid userId, CancellationToken cancellationToken = default);
    Task<PropertiesUserEntity> AddAsync(PropertiesUserEntity entity, CancellationToken cancellationToken = default);
    Task<PropertiesUserEntity> UpdateAsync(PropertiesUserEntity entity, CancellationToken cancellationToken = default);
}
