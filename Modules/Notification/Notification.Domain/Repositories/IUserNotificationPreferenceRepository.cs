using Notification.Domain.Entities;

namespace Notification.Domain.Repositories;

public interface IUserNotificationPreferenceRepository
{
    Task<UserNotificationPreferenceEntity?> GetByUserAndEntityTypeAsync(
        Guid userId,
        string entityType,
        CancellationToken cancellationToken = default);

    Task<UserNotificationPreferenceEntity> UpsertAsync(
        UserNotificationPreferenceEntity preference,
        CancellationToken cancellationToken = default);
}

