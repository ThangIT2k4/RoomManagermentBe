using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public class PasswordChangedConsumer(
    INotificationRepository notificationRepository,
    IUserNotificationRepository userNotificationRepository,
    IUnitOfWork unitOfWork,
    ILogger<PasswordChangedConsumer> logger): IConsumer<PasswordChangedEvent>
{
    public async Task Consume(ConsumeContext<PasswordChangedEvent> context)
    {
        var evt = context.Message;
        
        var notification = NotificationEntity.Create(
            title: "Thông báo thay đổi mật khẩu",
            content: $"Mật khẩu của bạn đã được thay đổi vào lúc {evt.ChangedAt:dd/MM/yyyy HH:mm}. Nếu bạn không thực hiện hành động này, vui lòng liên hệ với bộ phận hỗ trợ ngay lập tức.",
            type: "Security",
            createdAt: evt.ChangedAt
        );
        
        await notificationRepository.AddAsync(notification, context.CancellationToken);
        var userNotification = UserNotificationEntity.Create(evt.UserId, notification.Id);
        await userNotificationRepository.AddAsync(userNotification, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
        
        logger.LogInformation(
            "Password changed notification {NotificationId} created for user {UserId}",
            notification.Id, evt.UserId
        );
    }
}