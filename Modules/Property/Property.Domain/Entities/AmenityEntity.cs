namespace Property.Domain.Entities;

public sealed class AmenityEntity
{
    public Guid Id { get; }
    public string KeyCode { get; }
    public string Name { get; }
    public string? Category { get; }
    public string? Icon { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private AmenityEntity(Guid id, string keyCode, string name, string? category, string? icon, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        KeyCode = keyCode;
        Name = name;
        Category = category;
        Icon = icon;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static AmenityEntity FromPersistence(
        Guid id,
        string keyCode,
        string name,
        string? category,
        string? icon,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, keyCode, name, category, icon, createdAt, updatedAt);
}
