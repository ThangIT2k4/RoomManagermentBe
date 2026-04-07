namespace Property.Domain.Entities;

public sealed class PropertyStaffEntity
{
    public Guid Id { get; }
    public Guid PropertyId { get; }
    public Guid UserId { get; }
    public string RoleKey { get; }
    public DateTime AssignedAt { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private PropertyStaffEntity(
        Guid id,
        Guid propertyId,
        Guid userId,
        string roleKey,
        DateTime assignedAt,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        PropertyId = propertyId;
        UserId = userId;
        RoleKey = roleKey;
        AssignedAt = assignedAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static PropertyStaffEntity FromPersistence(
        Guid id,
        Guid propertyId,
        Guid userId,
        string roleKey,
        DateTime assignedAt,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, propertyId, userId, roleKey, assignedAt, createdAt, updatedAt);
}
