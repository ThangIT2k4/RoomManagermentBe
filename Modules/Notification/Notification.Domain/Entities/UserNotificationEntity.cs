namespace Notification.Domain.Entities;

/// <summary>
/// Domain entity: UserNotification (liên kết user – notification, trạng thái đã đọc).
/// Trong DDD, entity chỉ được tạo qua factory: Create (tạo mới) hoặc FromPersistence (tái tạo từ DB).
/// </summary>
public sealed class UserNotificationEntity
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid NotificationId { get; }
    public bool IsRead { get; }
    public DateTime? ReadAt { get; }
    public NotificationEntity? Notification { get; }

    private UserNotificationEntity(
        Guid id,
        Guid userId,
        Guid notificationId,
        bool isRead,
        DateTime? readAt,
        NotificationEntity? notification)
    {
        Id = id;
        UserId = userId;
        NotificationId = notificationId;
        IsRead = isRead;
        ReadAt = readAt;
        Notification = notification;
    }

    /// <summary>
    /// Factory: tạo user-notification mới (chưa đọc). Chỉ dùng khi đã có NotificationEntity.
    /// </summary>
    public static UserNotificationEntity Create(Guid userId, Guid notificationId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId cannot be empty.", nameof(notificationId));

        return new UserNotificationEntity(
            id: Guid.NewGuid(),
            userId,
            notificationId,
            isRead: false,
            readAt: null,
            notification: null);
    }

    /// <summary>
    /// Factory: tái tạo entity từ persistence (khi load từ DB). Chỉ dùng trong Infrastructure (mapper).
    /// </summary>
    public static UserNotificationEntity FromPersistence(
        Guid id,
        Guid userId,
        Guid notificationId,
        bool isRead,
        DateTime? readAt,
        NotificationEntity? notification = null)
    {
        return new UserNotificationEntity(id, userId, notificationId, isRead, readAt, notification);
    }
}
