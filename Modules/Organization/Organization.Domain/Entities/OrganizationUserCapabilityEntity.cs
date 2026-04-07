namespace Organization.Domain.Entities;

public sealed class OrganizationUserCapabilityEntity
{
    public Guid Id { get; }
    public Guid OrganizationUserId { get; }
    public Guid CapabilityId { get; }
    public bool Granted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private OrganizationUserCapabilityEntity(Guid id, Guid organizationUserId, Guid capabilityId, bool granted, DateTime createdAt)
    {
        Id = id;
        OrganizationUserId = organizationUserId;
        CapabilityId = capabilityId;
        Granted = granted;
        CreatedAt = createdAt;
    }

    public static OrganizationUserCapabilityEntity Create(Guid organizationUserId, Guid capabilityId, bool granted, DateTime now)
        => new(Guid.NewGuid(), organizationUserId, capabilityId, granted, now);

    public static OrganizationUserCapabilityEntity FromPersistence(
        Guid id,
        Guid organizationUserId,
        Guid capabilityId,
        bool granted,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new OrganizationUserCapabilityEntity(id, organizationUserId, capabilityId, granted, createdAt)
        {
            UpdatedAt = updatedAt
        };
    }

    public void SetGranted(bool granted, DateTime now)
    {
        Granted = granted;
        UpdatedAt = now;
    }
}
