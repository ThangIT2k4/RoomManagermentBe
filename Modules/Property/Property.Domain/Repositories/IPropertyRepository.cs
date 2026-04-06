using Property.Domain.Entities;

namespace Property.Domain.Repositories;

public interface IPropertyRepository
{
    Task<PropertyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PropertyEntity> AddAsync(PropertyEntity property, CancellationToken cancellationToken = default);
}

