using Organization.Domain.Entities;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Mapper;
using RoomManagerment.Organizations.DatabaseSpecific;
using RoomManagerment.Organizations.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Organization.Infrastructure.Repositories;

public sealed class OrganizationRepository(DataAccessAdapter adapter) : IOrganizationRepository
{
    public async Task<OrganizationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Organization.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<OrganizationEntity> AddAsync(OrganizationEntity organization, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Organizations.EntityClasses.OrganizationEntity
        {
            Id = organization.Id,
            Name = organization.Name,
            Code = organization.Code,
            Status = organization.Status,
            HasEverPaid = organization.HasEverPaid,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return organization;
    }
}

