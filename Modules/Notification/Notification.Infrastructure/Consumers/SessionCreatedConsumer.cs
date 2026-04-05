using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class SessionCreatedConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<SessionCreatedConsumer> logger) : IConsumer<SessionCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<SessionCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received SessionCreatedIntegrationEvent for user {UserId}", evt.UserId);

        var id = await ingestion.CreateAsync(
            evt.UserId,
            "Phiên đăng nhập mới",
            $"Một phiên mới đã được tạo lúc {evt.CreatedAt:dd/MM/yyyy HH:mm} (nguồn: {evt.SourceService}). Mã phiên: {evt.SessionId}.",
            "Security",
            evt.CreatedAt,
            context.CancellationToken);

        logger.LogInformation("Session created notification {NotificationId} for user {UserId}", id, evt.UserId);
    }
}
