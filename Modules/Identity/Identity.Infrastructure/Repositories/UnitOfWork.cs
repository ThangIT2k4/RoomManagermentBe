using Identity.Domain.Repositories;
using RoomManagerment.Identity.DatabaseSpecific;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace Identity.Infrastructure.Repositories;

public sealed class UnitOfWork(DataAccessAdapter adapter) : IUnitOfWork
{
    private readonly UnitOfWork2 _uow2 = new();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _uow2.Commit(adapter, true);
        return Task.FromResult(1);
    }
    
    public UnitOfWork2 Uow2 => _uow2;
}
