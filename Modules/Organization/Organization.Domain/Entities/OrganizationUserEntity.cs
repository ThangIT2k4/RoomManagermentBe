using Organization.Domain.Events;

namespace Organization.Domain.Entities;

public sealed class OrganizationUserEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid UserId { get; private set; }
    public Guid? RoleId { get; private set; }
    public bool IsActive { get; private set; }
    public string? InvitationEmail { get; private set; }
    public string? InvitationToken { get; private set; }
    public DateTime? InvitationExpiry { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? LastActiveAt { get; private set; }
    public DateTime? LastInactiveAt { get; private set; }

    private OrganizationUserEntity(Guid id, Guid organizationId, Guid userId, Guid? roleId, bool isActive, DateTime createdAt)
    {
        Id = id;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public static OrganizationUserEntity Create(Guid organizationId, Guid userId, Guid? roleId, DateTime now)
        => new(Guid.NewGuid(), organizationId, userId, roleId, true, now);

    public static OrganizationUserEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid userId,
        Guid? roleId,
        bool isActive,
        string? invitationEmail,
        string? invitationToken,
        DateTime? invitationExpiry,
        DateTime createdAt,
        DateTime? lastActiveAt,
        DateTime? lastInactiveAt)
    {
        return new OrganizationUserEntity(id, organizationId, userId, roleId, isActive, createdAt)
        {
            InvitationEmail = invitationEmail,
            InvitationToken = invitationToken,
            InvitationExpiry = invitationExpiry,
            LastActiveAt = lastActiveAt,
            LastInactiveAt = lastInactiveAt
        };
    }

    public static OrganizationUserEntity CreateInvitation(Guid organizationId, string email, Guid roleId, DateTime now, TimeSpan expiry)
    {
        return new OrganizationUserEntity(Guid.NewGuid(), organizationId, Guid.Empty, roleId, false, now)
        {
            InvitationEmail = email.Trim().ToLowerInvariant(),
            InvitationToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray()),
            InvitationExpiry = now.Add(expiry)
        };
    }

    public void ChangeRole(Guid roleId) => RoleId = roleId;

    public void Activate(DateTime now)
    {
        IsActive = true;
        LastActiveAt = now;
    }

    public void Deactivate(DateTime now)
    {
        IsActive = false;
        LastInactiveAt = now;
    }

    public void AcceptInvitation(Guid userId, DateTime now)
    {
        UserId = userId;
        IsActive = true;
        LastActiveAt = now;
        InvitationToken = null;
        InvitationExpiry = null;
    }

    public OrganizationMemberJoinedEvent ToJoinedEvent(DateTime now) => new(OrganizationId, UserId, RoleId, now);
}
