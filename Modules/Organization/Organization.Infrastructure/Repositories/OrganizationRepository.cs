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
            Status = (short)organization.Status,
            Email = organization.Email,
            Phone = organization.Phone,
            Mail = organization.Mail,
            TaxCode = organization.TaxCode,
            Address = organization.Address,
            HasEverPaid = organization.HasEverPaid,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return organization;
    }

    public async Task<OrganizationEntity> UpdateAsync(OrganizationEntity organization, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter)
            .Organization
            .Where(x => x.Id == organization.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (dal is null)
        {
            throw new InvalidOperationException($"Organization {organization.Id} not found.");
        }

        dal.Name = organization.Name;
        dal.Status = (short)organization.Status;
        dal.Email = organization.Email;
        dal.Phone = organization.Phone;
        dal.Mail = organization.Mail;
        dal.TaxCode = organization.TaxCode;
        dal.Address = organization.Address;
        dal.UpdatedAt = organization.UpdatedAt;
        await adapter.SaveEntityAsync(dal, false, false, cancellationToken);
        return organization;
    }
}

