using Auth.Domain.Common;
using Auth.Domain.Events;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class RoleEntity : AggregateRoot<Guid>
{
    public RoleKey KeyCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private RoleEntity() { }

    private RoleEntity(Guid id, RoleKey keyCode, string name, string? description, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        KeyCode = keyCode;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static RoleEntity Create(string keyCode, string name, string? description = null, DateTime? createdAt = null)
    {
        var entity = new RoleEntity(Guid.NewGuid(), RoleKey.Create(keyCode), name.Trim(), description?.Trim(), createdAt ?? DateTime.UtcNow, null);
        entity.AddDomainEvent(new RoleCreatedEvent(entity.Id, entity.KeyCode.Value, entity.Name, DateTimeOffset.UtcNow));
        return entity;
    }

    public static RoleEntity Reconstitute(Guid id, string keyCode, string name, string? description, DateTime createdAt, DateTime? updatedAt)
        => new(id, RoleKey.Create(keyCode), name, description, createdAt, updatedAt);

    public void Rename(string name, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        var trimmed = name.Trim();
        if (Name == trimmed) return;
        Name = trimmed;
        UpdatedAt = changedAt;
    }

    public void UpdateDescription(string? description, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        var trimmed = description?.Trim();
        if (Description == trimmed) return;
        Description = trimmed;
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

