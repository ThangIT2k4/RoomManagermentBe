namespace Notification.Application.Features.Notifications.GetNotificationById;

public sealed record NotificationDto(
    Guid Id,
    string Title,
    string Content,
    string? Type,
    DateTime CreatedAt);
