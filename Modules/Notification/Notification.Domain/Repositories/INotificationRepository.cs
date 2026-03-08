using Notification.Domain.Entities;

namespace Notification.Domain.Repositories;

public interface INotificationRepository
{
    Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationEntity> AddAsync(NotificationEntity notification, CancellationToken cancellationToken = default);
}
