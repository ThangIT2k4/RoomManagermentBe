using Property.Domain.Common;
using Property.Domain.Events;
using Property.Domain.ValueObjects;

namespace Property.Domain.Entities;

public sealed class PropertyEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; }
    public string? Code { get; private set; }
    public short Status { get; private set; }
    public int TotalUnits { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

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

        var entity = new PropertyEntity(Guid.NewGuid(), organizationId, name.Trim(), code is null ? null : PropertyCode.Create(code).Value, status, totalUnits, createdAt ?? DateTime.UtcNow, null);
        entity.AddDomainEvent(new PropertyCreatedEvent(entity.Id, entity.OrganizationId, entity.Name, DateTimeOffset.UtcNow));
        return entity;
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

