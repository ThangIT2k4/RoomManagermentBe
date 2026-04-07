namespace Property.Domain.Entities;

public sealed class PropertyTypeEntity
{
    public Guid Id { get; }
    public Guid? OrganizationId { get; }
    public string KeyCode { get; }
    public string Name { get; }
    public string? Description { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private PropertyTypeEntity(
        Guid id,
        Guid? organizationId,
        string keyCode,
        string name,
        string? description,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        KeyCode = keyCode;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static PropertyTypeEntity FromPersistence(
        Guid id,
        Guid? organizationId,
        string keyCode,
        string name,
        string? description,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, keyCode, name, description, createdAt, updatedAt);
}
