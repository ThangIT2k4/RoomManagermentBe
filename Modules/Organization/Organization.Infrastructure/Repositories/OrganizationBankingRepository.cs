using Organization.Domain.Entities;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Mapper;
using RoomManagerment.Organizations.DatabaseSpecific;
using RoomManagerment.Organizations.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Organization.Infrastructure.Repositories;

public sealed class OrganizationBankingRepository(DataAccessAdapter adapter) : IOrganizationBankingRepository
{
    public async Task<OrganizationBankingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationBanking.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<IReadOnlyList<OrganizationBankingEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var items = await new LinqMetaData(adapter).OrganizationBanking
            .Where(x => x.OrganizationId == organizationId && x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return items.Select(x => x.ToDomain()).ToList();
    }

    public async Task<OrganizationBankingEntity> AddAsync(OrganizationBankingEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Organizations.EntityClasses.OrganizationBankingEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            SepayBankId = entity.SepayBankId,
            AccountNumber = entity.AccountNumber,
            AccountHolderName = entity.AccountHolderName,
            BranchName = entity.BranchName,
            BranchCode = entity.BranchCode,
            SwiftCode = entity.SwiftCode,
            IsPrimary = entity.IsPrimary,
            CreatedAt = entity.CreatedAt
        };
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return entity;
    }

    public async Task<OrganizationBankingEntity> UpdateAsync(OrganizationBankingEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationBanking.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken)
                  ?? throw new InvalidOperationException($"OrganizationBanking {entity.Id} not found.");
        dal.IsPrimary = entity.IsPrimary;
        dal.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(dal, false, false, cancellationToken);
        return entity;
    }

    public async Task SoftDeleteAsync(Guid id, Guid deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationBanking.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (dal is null)
        {
            return;
        }

        dal.DeletedAt = deletedAt;
        dal.DeletedBy = deletedBy;
        dal.UpdatedAt = deletedAt;
        await adapter.SaveEntityAsync(dal, false, false, cancellationToken);
    }
}
