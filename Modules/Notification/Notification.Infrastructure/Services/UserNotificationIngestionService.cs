using Notification.Application.Services;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;

namespace Notification.Infrastructure.Services;

public sealed class UserNotificationIngestionService(
    IUserNotificationRepository userNotificationRepository,
    IUnitOfWork unitOfWork) : IUserNotificationIngestionService
{
    public async Task<Guid> CreateAsync(
        Guid userId,
        string title,
        string content,
        string notificationType,
        DateTime? createdAtUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = UserNotificationEntity.Create(
            userId,
            title,
            content,
            notificationType,
            createdAtUtc);

        await userNotificationRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
