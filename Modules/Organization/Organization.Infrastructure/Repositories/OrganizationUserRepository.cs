using Organization.Domain.Entities;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Mapper;
using RoomManagerment.Organizations.DatabaseSpecific;
using RoomManagerment.Organizations.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Organization.Infrastructure.Repositories;

public sealed class OrganizationUserRepository(DataAccessAdapter adapter) : IOrganizationUserRepository
{
    public async Task<OrganizationUserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUser.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<OrganizationUserEntity?> GetByOrgAndUserAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUser
            .Where(x => x.OrganizationId == organizationId && x.UserId == userId && x.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<OrganizationUserEntity?> GetByInvitationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUser
            .Where(x => x.InvitationToken == token && x.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<IReadOnlyList<OrganizationUserEntity>> GetPagedByOrgAsync(Guid organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = Math.Max(0, (page - 1) * pageSize);
        var items = await new LinqMetaData(adapter).OrganizationUser
            .Where(x => x.OrganizationId == organizationId && x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return items.Select(x => x.ToDomain()).ToList();
    }

    public Task<long> CountByOrgAsync(Guid organizationId, CancellationToken cancellationToken = default)
        => new LinqMetaData(adapter).OrganizationUser
            .Where(x => x.OrganizationId == organizationId && x.DeletedAt == null)
            .LongCountAsync(cancellationToken);

    public async Task<OrganizationUserEntity> AddAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = new RoomManagerment.Organizations.EntityClasses.OrganizationUserEntity
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            UserId = entity.UserId,
            RoleId = entity.RoleId,
            IsActive = entity.IsActive,
            InvitationEmail = entity.InvitationEmail,
            InvitationToken = entity.InvitationToken,
            InvitationExpiry = entity.InvitationExpiry,
            CreatedAt = entity.CreatedAt,
            LastActiveAt = entity.LastActiveAt,
            LastInactiveAt = entity.LastInactiveAt
        };
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        return entity;
    }

    public async Task<OrganizationUserEntity> UpdateAsync(OrganizationUserEntity entity, CancellationToken cancellationToken = default)
    {
        var dal = await new LinqMetaData(adapter).OrganizationUser.Where(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken)
                  ?? throw new InvalidOperationException($"Không tìm thấy thành viên tổ chức {entity.Id}.");
        dal.RoleId = entity.RoleId;
        dal.IsActive = entity.IsActive;
        dal.InvitationToken = entity.InvitationToken;
        dal.InvitationEmail = entity.InvitationEmail;
        dal.InvitationExpiry = entity.InvitationExpiry;
        dal.LastActiveAt = entity.LastActiveAt;
        dal.LastInactiveAt = entity.LastInactiveAt;
        dal.UserId = entity.UserId;
        dal.UpdatedAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(dal, false, false, cancellationToken);
        return entity;
    }
}
