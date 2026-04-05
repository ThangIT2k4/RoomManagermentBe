using DomainNotificationEntity = Notification.Domain.Entities.NotificationEntity;
using DomainUserNotificationEntity = Notification.Domain.Entities.UserNotificationEntity;
using DomainUserNotificationPreferenceEntity = global::Notification.Domain.Entities.UserNotificationPreferenceEntity;
using DalNotificationEntity = RoomManagerment.Notification.EntityClasses.NotificationEntity;
using DalUserNotificationPreferenceEntity = RoomManagerment.Notification.EntityClasses.UserNotificationPreferenceEntity;

namespace Notification.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainNotificationEntity ToDomain(this DalNotificationEntity dal)
    {
        return DomainNotificationEntity.FromPersistence(
            dal.Id,
            dal.UserId,
            dal.NotificationChannelId,
            dal.Title ?? string.Empty,
            dal.Message ?? string.Empty,
            dal.Type,
            dal.CreatedAt,
            dal.IsRead,
            dal.ReadAt);
    }

    public static DomainUserNotificationEntity ToUserNotificationDomain(this DalNotificationEntity dal)
    {
        return DomainUserNotificationEntity.FromPersistence(
            dal.Id,
            dal.UserId,
            dal.Title ?? string.Empty,
            dal.Message ?? string.Empty,
            dal.Type,
            dal.CreatedAt,
            dal.IsRead,
            dal.ReadAt);
    }

    public static DomainUserNotificationPreferenceEntity ToDomain(this DalUserNotificationPreferenceEntity dal)
    {
        return DomainUserNotificationPreferenceEntity.FromPersistence(
            dal.Id,
            dal.UserId,
            dal.EntityType,
            dal.InAppEnabled,
            dal.EmailEnabled,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

}
