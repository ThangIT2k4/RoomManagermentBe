namespace Notification.Domain.Entities;

/// <summary>
/// Domain entity mirrored from DAL UserNotificationPreference row.
/// </summary>
public sealed class UserNotificationPreferenceEntity
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string EntityType { get; }
    public bool InAppEnabled { get; }
    public bool EmailEnabled { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private UserNotificationPreferenceEntity(
        Guid id,
        Guid userId,
        string entityType,
        bool inAppEnabled,
        bool emailEnabled,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        UserId = userId;
        EntityType = entityType;
        InAppEnabled = inAppEnabled;
        EmailEnabled = emailEnabled;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserNotificationPreferenceEntity Create(
        Guid userId,
        string entityType,
        bool inAppEnabled = true,
        bool emailEnabled = false,
        DateTime? createdAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("EntityType cannot be empty.", nameof(entityType));

        return new UserNotificationPreferenceEntity(
            Guid.NewGuid(),
            userId,
            entityType.Trim(),
            inAppEnabled,
            emailEnabled,
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static UserNotificationPreferenceEntity FromPersistence(
        Guid id,
        Guid userId,
        string entityType,
        bool inAppEnabled,
        bool emailEnabled,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new UserNotificationPreferenceEntity(
            id,
            userId,
            entityType ?? string.Empty,
            inAppEnabled,
            emailEnabled,
            createdAt,
            updatedAt);
    }
}

