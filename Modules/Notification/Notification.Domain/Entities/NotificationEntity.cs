namespace Notification.Domain.Entities;

/// <summary>
/// Domain entity: Notification.
/// Trong DDD, entity chỉ được tạo qua factory: Create (tạo mới) hoặc FromPersistence (tái tạo từ DB).
/// </summary>
public sealed class NotificationEntity
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid NotificationChannelId { get; }
    public string Title { get; }
    public string Content { get; }
    public string? Type { get; }
    public DateTime CreatedAt { get; }
    public bool IsRead { get; }
    public DateTime? ReadAt { get; }

    private NotificationEntity(
        Guid id,
        Guid userId,
        Guid notificationChannelId,
        string title,
        string content,
        string? type,
        DateTime createdAt,
        bool isRead,
        DateTime? readAt)
    {
        Id = id;
        UserId = userId;
        NotificationChannelId = notificationChannelId;
        Title = title;
        Content = content;
        Type = type;
        CreatedAt = createdAt;
        IsRead = isRead;
        ReadAt = readAt;
    }

    /// <summary>
    /// Factory: tạo notification mới. Mọi invariant và giá trị mặc định nằm trong domain.
    /// </summary>
    public static NotificationEntity Create(
        Guid userId,
        string title,
        string content,
        string? type = null,
        DateTime? createdAt = null,
        Guid? notificationChannelId = null,
        bool isRead = false,
        DateTime? readAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        return new NotificationEntity(
            id: Guid.NewGuid(),
            userId: userId,
            notificationChannelId: notificationChannelId ?? Guid.Empty,
            title: title.Trim(),
            content: content.Trim(),
            type: string.IsNullOrWhiteSpace(type) ? "Info" : type.Trim(),
            createdAt: createdAt ?? DateTime.UtcNow,
            isRead: isRead,
            readAt: readAt);
    }

    /// <summary>
    /// Factory: tái tạo entity từ persistence (khi load từ DB). Chỉ dùng trong Infrastructure (mapper).
    /// </summary>
    public static NotificationEntity FromPersistence(
        Guid id,
        Guid userId,
        Guid notificationChannelId,
        string title,
        string content,
        string? type,
        DateTime createdAt,
        bool isRead,
        DateTime? readAt)
    {
        return new NotificationEntity(
            id,
            userId,
            notificationChannelId,
            title ?? string.Empty,
            content ?? string.Empty,
            type,
            createdAt,
            isRead,
            readAt);
    }
}
