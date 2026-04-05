using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class SessionRevokedConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<SessionRevokedConsumer> logger) : IConsumer<SessionRevokedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<SessionRevokedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received SessionRevokedIntegrationEvent for user {UserId}", evt.UserId);

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Phiên đăng nhập đã kết thúc",
            $"Phiên {evt.SessionId} đã được thu hồi lúc {evt.RevokedAt:dd/MM/yyyy HH:mm} (nguồn: {evt.SourceService}).",
            "Security",
            evt.RevokedAt,
            context.CancellationToken);

        logger.LogInformation("Session revoked notification {NotificationId} for user {UserId}", id, evt.UserId);
    }
}
