using DalOrganizationEntity = RoomManagerment.Organizations.EntityClasses.OrganizationEntity;
using DomainOrganizationEntity = Organization.Domain.Entities.OrganizationEntity;

namespace Organization.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainOrganizationEntity ToDomain(this DalOrganizationEntity dal)
    {
        return DomainOrganizationEntity.FromPersistence(
            dal.Id,
            dal.Name ?? string.Empty,
            dal.Code,
            dal.Status,
            dal.HasEverPaid,
            dal.CreatedAt,
            dal.UpdatedAt);
    }
}

