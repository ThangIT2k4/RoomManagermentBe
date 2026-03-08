using Notification.Domain.Entities;

namespace Notification.Domain.Repositories;

public interface IUserNotificationRepository
{
    Task<UserNotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserNotificationEntity?> GetByUserAndNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserNotificationEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, bool? isRead = null, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserNotificationEntity> AddAsync(UserNotificationEntity userNotification, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid userNotificationId, Guid userId, CancellationToken cancellationToken = default);
}
