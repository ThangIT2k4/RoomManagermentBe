namespace Organization.Domain.Entities;

public sealed class OrganizationEntity
{
    public Guid Id { get; }
    public string Name { get; }
    public string? Code { get; }
    public short Status { get; }
    public bool HasEverPaid { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private OrganizationEntity(Guid id, string name, string? code, short status, bool hasEverPaid, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        Code = code;
        Status = status;
        HasEverPaid = hasEverPaid;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static OrganizationEntity Create(string name, short status = 1, string? code = null, bool hasEverPaid = false, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return new OrganizationEntity(Guid.NewGuid(), name.Trim(), code?.Trim(), status, hasEverPaid, createdAt ?? DateTime.UtcNow, null);
    }

    public static OrganizationEntity FromPersistence(
        Guid id,
        string name,
        string? code,
        short status,
        bool hasEverPaid,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new OrganizationEntity(id, name ?? string.Empty, code, status, hasEverPaid, createdAt, updatedAt);
    }
}

