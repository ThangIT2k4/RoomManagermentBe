using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class UserLoggedInConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<UserLoggedInConsumer> logger) : IConsumer<UserLoggedInEvent>
{
    public async Task Consume(ConsumeContext<UserLoggedInEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received UserLoggedInEvent for user {UserId}", evt.UserId);

        var ip = string.IsNullOrWhiteSpace(evt.IpAddress) ? "không xác định" : evt.IpAddress;

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Đăng nhập thành công",
            $"Tài khoản {evt.Username} vừa đăng nhập lúc {evt.LoggedInAt:dd/MM/yyyy HH:mm} từ địa chỉ IP {ip}. Nếu không phải bạn, hãy đổi mật khẩu ngay.",
            "Security",
            evt.LoggedInAt,
            context.CancellationToken);

        logger.LogInformation("Login notification {NotificationId} created for user {UserId}", id, evt.UserId);
    }
}
