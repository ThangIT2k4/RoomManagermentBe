using Notification.Domain.Entities;
using RoomManagerment.Notification.EntityClasses;

namespace Notification.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static Notification.Domain.Entities.NotificationEntity ToDomain(this RoomManagerment.Notification.EntityClasses.NotificationEntity dal)
    {
        if (dal is null) return null!;
        return new Notification.Domain.Entities.NotificationEntity
        {
            Id = dal.Id,
            Title = dal.Title ?? string.Empty,
            Content = dal.Content ?? string.Empty,
            Type = dal.Type,
            CreatedAt = dal.CreatedAt
        };
    }

    public static Notification.Domain.Entities.UserNotificationEntity ToDomain(this RoomManagerment.Notification.EntityClasses.UserNotificationEntity dal, Notification.Domain.Entities.NotificationEntity? notification = null)
    {
        if (dal is null) return null!;
        return new Notification.Domain.Entities.UserNotificationEntity
        {
            Id = dal.Id,
            UserId = dal.UserId,
            NotificationId = dal.NotificationId,
            IsRead = dal.IsRead,
            ReadAt = dal.ReadAt,
            Notification = notification
        };
    }
}
