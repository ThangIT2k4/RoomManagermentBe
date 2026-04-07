using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface IDocumentRepository
{
    Task<DocumentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DocumentEntity> AddAsync(DocumentEntity entity, CancellationToken cancellationToken = default);
    Task<DocumentEntity> UpdateAsync(DocumentEntity entity, CancellationToken cancellationToken = default);
}
