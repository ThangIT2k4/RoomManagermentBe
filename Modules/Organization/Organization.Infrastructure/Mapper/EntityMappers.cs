using DalOrganizationEntity = RoomManagerment.Organizations.EntityClasses.OrganizationEntity;
using DalOrganizationBankingEntity = RoomManagerment.Organizations.EntityClasses.OrganizationBankingEntity;
using DalOrganizationUserEntity = RoomManagerment.Organizations.EntityClasses.OrganizationUserEntity;
using DalOrganizationUserCapabilityEntity = RoomManagerment.Organizations.EntityClasses.OrganizationUserCapabilityEntity;
using DomainOrganizationBankingEntity = Organization.Domain.Entities.OrganizationBankingEntity;
using DomainOrganizationEntity = Organization.Domain.Entities.OrganizationEntity;
using DomainOrganizationUserCapabilityEntity = Organization.Domain.Entities.OrganizationUserCapabilityEntity;
using DomainOrganizationUserEntity = Organization.Domain.Entities.OrganizationUserEntity;

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
            dal.UpdatedAt,
            dal.Email,
            dal.Phone,
            dal.Mail,
            dal.TaxCode,
            dal.Address);
    }

    public static DomainOrganizationUserEntity ToDomain(this DalOrganizationUserEntity dal)
        => DomainOrganizationUserEntity.FromPersistence(
            dal.Id,
            dal.OrganizationId,
            dal.UserId,
            dal.RoleId,
            dal.IsActive,
            dal.InvitationEmail,
            dal.InvitationToken,
            dal.InvitationExpiry,
            dal.CreatedAt,
            dal.LastActiveAt,
            dal.LastInactiveAt);

    public static DomainOrganizationBankingEntity ToDomain(this DalOrganizationBankingEntity dal)
        => DomainOrganizationBankingEntity.FromPersistence(
            dal.Id,
            dal.OrganizationId,
            dal.SepayBankId,
            dal.AccountNumber,
            dal.AccountHolderName,
            dal.BranchName,
            dal.BranchCode,
            dal.SwiftCode,
            dal.IsPrimary,
            dal.CreatedAt);

    public static DomainOrganizationUserCapabilityEntity ToDomain(this DalOrganizationUserCapabilityEntity dal)
        => DomainOrganizationUserCapabilityEntity.FromPersistence(
            dal.Id,
            dal.OrganizationUserId,
            dal.CapabilityId,
            dal.Granted,
            dal.CreatedAt,
            dal.UpdatedAt);
}

