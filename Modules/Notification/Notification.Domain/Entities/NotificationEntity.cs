namespace Notification.Domain.Entities;

/// <summary>
/// Domain entity: Notification.
/// Trong DDD, entity chỉ được tạo qua factory: Create (tạo mới) hoặc FromPersistence (tái tạo từ DB).
/// </summary>
public sealed class NotificationEntity
{
    public Guid Id { get; }
    public string Title { get; }
    public string Content { get; }
    public string? Type { get; }
    public DateTime CreatedAt { get; }

    private NotificationEntity(Guid id, string title, string content, string? type, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Content = content;
        Type = type;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory: tạo notification mới. Mọi invariant và giá trị mặc định nằm trong domain.
    /// </summary>
    public static NotificationEntity Create(string title, string content, string? type = null, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.", nameof(content));

        return new NotificationEntity(
            id: Guid.NewGuid(),
            title: title.Trim(),
            content: content.Trim(),
            type: string.IsNullOrWhiteSpace(type) ? "Info" : type.Trim(),
            createdAt: createdAt ?? DateTime.UtcNow);
    }

    /// <summary>
    /// Factory: tái tạo entity từ persistence (khi load từ DB). Chỉ dùng trong Infrastructure (mapper).
    /// </summary>
    public static NotificationEntity FromPersistence(Guid id, string title, string content, string? type, DateTime createdAt)
    {
        return new NotificationEntity(id, title ?? string.Empty, content ?? string.Empty, type, createdAt);
    }
}
