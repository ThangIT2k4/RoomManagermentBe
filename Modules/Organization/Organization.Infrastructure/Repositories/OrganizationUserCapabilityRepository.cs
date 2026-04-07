using Organization.Domain.Entities;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Mapper;
using RoomManagerment.Organizations.DatabaseSpecific;
using RoomManagerment.Organizations.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Organization.Infrastructure.Repositories;

public sealed class OrganizationUserCapabilityRepository(DataAccessAdapter adapter) : IOrganizationUserCapabilityRepository
{
    public async Task<OrganizationUserCapabilityEntity?> GetByOrganizationUserAndCapabilityAsync(Guid organizationUserId, Guid capabilityId, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUserCapability
            .Where(x => x.OrganizationUserId == organizationUserId && x.CapabilityId == capabilityId)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<OrganizationUserCapabilityEntity> UpsertAsync(OrganizationUserCapabilityEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUserCapability
            .Where(x => x.OrganizationUserId == entity.OrganizationUserId && x.CapabilityId == entity.CapabilityId)
            .FirstOrDefaultAsync(cancellationToken);

        if (dal is null)
        {
            dal = new RoomManagerment.Organizations.EntityClasses.OrganizationUserCapabilityEntity
            {
                Id = entity.Id,
                OrganizationUserId = entity.OrganizationUserId,
                CapabilityId = entity.CapabilityId,
                Granted = entity.Granted,
                CreatedAt = entity.CreatedAt
            };
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            return entity;
        }

        dal.Granted = entity.Granted;
        dal.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(dal, false, false, cancellationToken);
        return entity;
    }
}
