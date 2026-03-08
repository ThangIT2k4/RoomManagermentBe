namespace Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;

public sealed record UserNotificationListItemDto(
    Guid Id,
    Guid UserId,
    Guid NotificationId,
    string Title,
    string Content,
    string? Type,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);
