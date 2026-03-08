namespace Notification.Domain.Entities;

public sealed class UserNotificationEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid NotificationId { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public NotificationEntity? Notification { get; init; }
}
