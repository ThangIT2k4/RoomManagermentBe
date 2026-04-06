namespace Property.Domain.Entities;

public sealed class PropertyEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string Name { get; }
    public string? Code { get; }
    public short Status { get; }
    public int TotalUnits { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private PropertyEntity(
        Guid id,
        Guid organizationId,
        string name,
        string? code,
        short status,
        int totalUnits,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Code = code;
        Status = status;
        TotalUnits = totalUnits;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static PropertyEntity Create(
        Guid organizationId,
        string name,
        int totalUnits,
        short status = 1,
        string? code = null,
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return new PropertyEntity(Guid.NewGuid(), organizationId, name.Trim(), code?.Trim(), status, totalUnits, createdAt ?? DateTime.UtcNow, null);
    }

    public static PropertyEntity FromPersistence(
        Guid id,
        Guid organizationId,
        string name,
        string? code,
        short status,
        int totalUnits,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new PropertyEntity(id, organizationId, name ?? string.Empty, code, status, totalUnits, createdAt, updatedAt);
    }
}

