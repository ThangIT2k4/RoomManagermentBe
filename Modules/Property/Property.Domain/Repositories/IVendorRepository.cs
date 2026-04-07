using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IVendorRepository
{
    Task<VendorEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VendorEntity> AddAsync(VendorEntity entity, CancellationToken cancellationToken = default);
    Task<VendorEntity> UpdateAsync(VendorEntity entity, CancellationToken cancellationToken = default);
}
