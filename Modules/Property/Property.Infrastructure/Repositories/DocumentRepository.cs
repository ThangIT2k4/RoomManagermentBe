using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using RoomManagerment.Property.EntityClasses;
using RoomManagerment.Property.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class DocumentRepository(DataAccessAdapter adapter) : IDocumentRepository
{
    public async Task<DocumentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        return await linq.Document.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DocumentEntity> AddAsync(DocumentEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }

    public async Task<DocumentEntity> UpdateAsync(DocumentEntity entity, CancellationToken cancellationToken = default)
    {
        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return entity;
    }
}
