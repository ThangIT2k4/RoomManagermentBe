using DalPropertyEntity = RoomManagerment.Property.EntityClasses.PropertyEntity;
using DomainPropertyEntity = Property.Domain.Entities.PropertyEntity;

namespace Property.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainPropertyEntity ToDomain(this DalPropertyEntity dal)
    {
        return DomainPropertyEntity.FromPersistence(
            dal.Id,
            dal.OrganizationId,
            dal.Name ?? string.Empty,
            dal.Code,
            dal.Status,
            dal.TotalUnits,
            dal.CreatedAt,
            dal.UpdatedAt);
    }
}

