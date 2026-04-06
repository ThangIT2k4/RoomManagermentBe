using CRM.Domain.Repositories;
using RoomManagerment.CRM.DatabaseSpecific;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace CRM.Infrastructure.Repositories;

public sealed class UnitOfWork(DataAccessAdapter adapter) : IUnitOfWork
{
    private readonly UnitOfWork2 _uow2 = new();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _uow2.Commit(adapter, true);
        return Task.FromResult(1);
    }
}

