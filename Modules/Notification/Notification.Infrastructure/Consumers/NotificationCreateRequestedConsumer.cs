using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class NotificationCreateRequestedConsumer(
    INotificationRepository notificationRepository,
    IUserNotificationRepository userNotificationRepository,
    IUnitOfWork unitOfWork,
    ILogger<NotificationCreateRequestedConsumer> logger) : IConsumer<NotificationCreateRequestedEvent>
{
    public async Task Consume(ConsumeContext<NotificationCreateRequestedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation(
            "Received NotificationCreateRequestedEvent for user {UserId} from {Source}",
            evt.RecipientUserId, evt.SourceService);

        var notification = NotificationEntity.Create(
            evt.Title,
            evt.Message,
            evt.Type,
            evt.CreatedAt);

        await notificationRepository.AddAsync(notification, context.CancellationToken);

        var userNotification = UserNotificationEntity.Create(evt.RecipientUserId, notification.Id);

        await userNotificationRepository.AddAsync(userNotification, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "Notification {NotificationId} created for user {UserId}",
            notification.Id, evt.RecipientUserId);
    }
}
