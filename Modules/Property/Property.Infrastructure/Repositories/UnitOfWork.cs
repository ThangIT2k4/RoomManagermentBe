using Property.Domain.Repositories;
using RoomManagerment.Property.DatabaseSpecific;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace Property.Infrastructure.Repositories;

public sealed class UnitOfWork(DataAccessAdapter adapter) : IUnitOfWork
{
    private readonly UnitOfWork2 _uow2 = new();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _uow2.Commit(adapter, true);
        return Task.FromResult(1);
    }
}

