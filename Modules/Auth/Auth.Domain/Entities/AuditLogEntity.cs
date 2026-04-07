using Auth.Domain.Common;

namespace Auth.Domain.Entities;

public sealed class AuditLogEntity : AggregateRoot<Guid>
{
    public Guid? ActorId { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public string? BeforeJson { get; private set; }
    public string? AfterJson { get; private set; }
    public string? ChangesJson { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AuditLogEntity() { }

    private AuditLogEntity(Guid id, Guid? actorId, Guid? organizationId, string action, string entityType, Guid? entityId, string? beforeJson, string? afterJson, string? changesJson, string? ipAddress, string? userAgent, DateTime createdAt)
    {
        Id = id;
        ActorId = actorId;
        OrganizationId = organizationId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        BeforeJson = beforeJson;
        AfterJson = afterJson;
        ChangesJson = changesJson;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = createdAt;
    }

    public static AuditLogEntity Record(Guid? actorId, Guid? organizationId, string action, string entityType, Guid? entityId = null, string? beforeJson = null, string? afterJson = null, string? changesJson = null, string? ipAddress = null, string? userAgent = null, DateTime? createdAt = null)
    {
        var normalizedAction = action.Trim();
        var normalizedEntityType = entityType.Trim();

        if (string.IsNullOrWhiteSpace(normalizedAction))
        {
            throw new ArgumentException("Action cannot be empty.", nameof(action));
        }

        if (string.IsNullOrWhiteSpace(normalizedEntityType))
        {
            throw new ArgumentException("Entity type cannot be empty.", nameof(entityType));
        }

        return new AuditLogEntity(
            Guid.NewGuid(),
            actorId,
            organizationId,
            normalizedAction,
            normalizedEntityType,
            entityId,
            beforeJson?.Trim(),
            afterJson?.Trim(),
            changesJson?.Trim(),
            ipAddress?.Trim(),
            userAgent?.Trim(),
            createdAt ?? DateTime.UtcNow);
    }

    public static AuditLogEntity Reconstitute(
        Guid id,
        Guid? actorId,
        Guid? organizationId,
        string action,
        string entityType,
        Guid? entityId,
        string? beforeJson,
        string? afterJson,
        string? changesJson,
        string? ipAddress,
        string? userAgent,
        DateTime createdAt)
        => new(
            id,
            actorId,
            organizationId,
            action,
            entityType,
            entityId,
            beforeJson,
            afterJson,
            changesJson,
            ipAddress,
            userAgent,
            createdAt);
}

