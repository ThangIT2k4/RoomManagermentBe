using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

/// <summary>
/// Nhận UserRegisteredEvent từ RabbitMQ (Identity publish sau khi đăng ký thành công).
/// Tự động tạo notification chào mừng cho user mới.
/// Queue: notification-user-registered
/// </summary>
public sealed class UserRegisteredConsumer(
    INotificationRepository notificationRepository,
    IUserNotificationRepository userNotificationRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received UserRegisteredEvent for user {UserId}", evt.UserId);

        var notification = NotificationEntity.Create(
            title: "Chào mừng bạn đến với RoomManagerment!",
            content: $"Xin chào {evt.Username}, tài khoản của bạn đã được tạo thành công vào lúc {evt.RegisteredAt:dd/MM/yyyy HH:mm}.",
            type: "Info");

        await notificationRepository.AddAsync(notification, context.CancellationToken);

        var userNotification = UserNotificationEntity.Create(evt.UserId, notification.Id);

        await userNotificationRepository.AddAsync(userNotification, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "Welcome notification {NotificationId} created for new user {UserId}",
            notification.Id, evt.UserId);
    }
}
