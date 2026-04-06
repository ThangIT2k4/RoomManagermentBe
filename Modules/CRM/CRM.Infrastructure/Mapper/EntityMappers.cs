using DalLeadEntity = RoomManagerment.CRM.EntityClasses.LeadEntity;
using DomainLeadEntity = CRM.Domain.Entities.LeadEntity;

namespace CRM.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainLeadEntity ToDomain(this DalLeadEntity dal)
    {
        return DomainLeadEntity.FromPersistence(dal.Id, dal.OrganizationId, dal.FullName, dal.Status ?? string.Empty, dal.CreatedAt, dal.UpdatedAt);
    }
}

