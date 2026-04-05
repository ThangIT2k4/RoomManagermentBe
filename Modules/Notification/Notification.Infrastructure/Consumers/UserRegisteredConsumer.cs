using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class UserRegisteredConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received UserRegisteredEvent for user {UserId}", evt.UserId);

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Chào mừng bạn đến với RoomManagerment!",
            $"Xin chào {evt.Username}, tài khoản của bạn đã được tạo thành công vào lúc {evt.RegisteredAt:dd/MM/yyyy HH:mm}.",
            "Info",
            evt.RegisteredAt,
            context.CancellationToken);

        logger.LogInformation(
            "Welcome notification {NotificationId} created for new user {UserId}",
            id, evt.UserId);
    }
}
