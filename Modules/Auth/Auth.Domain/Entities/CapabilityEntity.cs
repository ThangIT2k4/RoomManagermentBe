using Auth.Domain.Common;
using Auth.Domain.Events;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class CapabilityEntity : AggregateRoot<Guid>
{
    public CapabilityKey KeyCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Category { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CapabilityEntity() { }

    private CapabilityEntity(Guid id, CapabilityKey keyCode, string name, string? description, string? category, int displayOrder, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        KeyCode = keyCode;
        Name = name;
        Description = description;
        Category = category;
        DisplayOrder = displayOrder;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CapabilityEntity Create(string keyCode, string name, string? description = null, string? category = null, int displayOrder = 0, DateTime? createdAt = null)
    {
        var entity = new CapabilityEntity(Guid.NewGuid(), CapabilityKey.Create(keyCode), name.Trim(), description?.Trim(), category?.Trim(), displayOrder, createdAt ?? DateTime.UtcNow, null);
        entity.AddDomainEvent(new CapabilityCreatedEvent(entity.Id, entity.KeyCode.Value, entity.Name, DateTimeOffset.UtcNow));
        return entity;
    }

    public static CapabilityEntity Reconstitute(Guid id, string keyCode, string name, string? description, string? category, int displayOrder, DateTime createdAt, DateTime? updatedAt)
        => new(id, CapabilityKey.Create(keyCode), name, description, category, displayOrder, createdAt, updatedAt);

    public void Rename(string name, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        var trimmed = name.Trim();
        if (Name == trimmed) return;
        Name = trimmed;
        UpdatedAt = changedAt;
    }

    public void UpdateDetails(string? description, string? category, int displayOrder, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        Description = description?.Trim();
        Category = category?.Trim();
        DisplayOrder = displayOrder;
        UpdatedAt = changedAt;
    }

    private static void EnsureUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Timestamp must be in UTC.", nameof(value));
        }
    }
}

