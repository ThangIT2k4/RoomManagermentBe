using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class PasswordChangedConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<PasswordChangedConsumer> logger) : IConsumer<PasswordChangedEvent>
{
    public async Task Consume(ConsumeContext<PasswordChangedEvent> context)
    {
        var evt = context.Message;

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Thông báo thay đổi mật khẩu",
            $"Mật khẩu của bạn đã được thay đổi vào lúc {evt.ChangedAt:dd/MM/yyyy HH:mm} (nguồn: {evt.SourceService}). Nếu bạn không thực hiện hành động này, vui lòng liên hệ hỗ trợ ngay.",
            "Security",
            evt.ChangedAt,
            context.CancellationToken);

        logger.LogInformation(
            "Password changed notification {NotificationId} created for user {UserId}",
            id, evt.UserId);
    }
}
