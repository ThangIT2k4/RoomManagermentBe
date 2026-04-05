namespace Notification.Domain.Entities;

/// <summary>
/// Domain entity: UserNotification (một bản ghi notifications gắn user và trạng thái đã đọc).
/// Trong DDD, entity chỉ được tạo qua factory: Create (tạo mới) hoặc FromPersistence (tái tạo từ DB).
/// </summary>
public sealed class UserNotificationEntity
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Title { get; }
    public string Content { get; }
    public string? Type { get; }
    public DateTime CreatedAt { get; }
    public bool IsRead { get; }
    public DateTime? ReadAt { get; }

    private UserNotificationEntity(
        Guid id,
        Guid userId,
        string title,
        string content,
        string? type,
        DateTime createdAt,
        bool isRead,
        DateTime? readAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Content = content;
        Type = type;
        CreatedAt = createdAt;
        IsRead = isRead;
        ReadAt = readAt;
    }

    /// <summary>
    /// Factory: tạo user-notification mới.
    /// </summary>
    public static UserNotificationEntity Create(
        Guid userId,
        string title,
        string content,
        string? type = null,
        DateTime? createdAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        return new UserNotificationEntity(
            id: Guid.NewGuid(),
            userId,
            title: title.Trim(),
            content: content.Trim(),
            type: string.IsNullOrWhiteSpace(type) ? "Info" : type.Trim(),
            createdAt: createdAt ?? DateTime.UtcNow,
            isRead: false,
            readAt: null);
    }

    /// <summary>
    /// Factory: tái tạo entity từ persistence (khi load từ DB). Chỉ dùng trong Infrastructure (mapper).
    /// </summary>
    public static UserNotificationEntity FromPersistence(
        Guid id,
        Guid userId,
        string title,
        string content,
        string? type,
        DateTime createdAt,
        bool isRead,
        DateTime? readAt)
    {
        return new UserNotificationEntity(
            id,
            userId,
            title ?? string.Empty,
            content ?? string.Empty,
            type,
            createdAt,
            isRead,
            readAt);
    }
}
