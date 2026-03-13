using Notification.Domain.Entities;
using RoomManagerment.Notification.EntityClasses;

namespace Notification.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static Notification.Domain.Entities.NotificationEntity ToDomain(this RoomManagerment.Notification.EntityClasses.NotificationEntity dal)
    {
        if (dal is null) return null!;
        return Notification.Domain.Entities.NotificationEntity.FromPersistence(
            dal.Id,
            dal.Title ?? string.Empty,
            dal.Content ?? string.Empty,
            dal.Type,
            dal.CreatedAt);
    }

    public static Notification.Domain.Entities.UserNotificationEntity ToDomain(this RoomManagerment.Notification.EntityClasses.UserNotificationEntity dal, Notification.Domain.Entities.NotificationEntity? notification = null)
    {
        if (dal is null) return null!;
        return Notification.Domain.Entities.UserNotificationEntity.FromPersistence(
            dal.Id,
            dal.UserId,
            dal.NotificationId,
            dal.IsRead,
            dal.ReadAt,
            notification);
    }
}
