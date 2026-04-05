using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class UserEmailVerifiedConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<UserEmailVerifiedConsumer> logger) : IConsumer<UserEmailVerifiedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserEmailVerifiedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received UserEmailVerifiedIntegrationEvent for user {UserId}", evt.UserId);

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Xác minh email thành công",
            $"Địa chỉ {evt.Email} đã được xác minh lúc {evt.VerifiedAt:dd/MM/yyyy HH:mm} (nguồn: {evt.SourceService}).",
            "Info",
            evt.VerifiedAt,
            context.CancellationToken);

        logger.LogInformation("Email verified notification {NotificationId} for user {UserId}", id, evt.UserId);
    }
}
