using System.Text;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Services;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class NotificationCreateRequestedConsumer(
    IUserNotificationIngestionService ingestion,
    ILogger<NotificationCreateRequestedConsumer> logger) : IConsumer<NotificationCreateRequestedEvent>
{
    public async Task Consume(ConsumeContext<NotificationCreateRequestedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation(
            "Received NotificationCreateRequestedEvent for user {UserId} from {Source}",
            evt.RecipientUserId, evt.SourceService);

        var content = BuildContent(evt.Message, evt.Metadata);

        var id = await ingestion.CreateAsync(
            evt.RecipientUserId,
            evt.Title,
            content,
            evt.Type,
            evt.CreatedAt,
            context.CancellationToken);

        logger.LogInformation(
            "Notification {NotificationId} created for user {UserId}",
            id, evt.RecipientUserId);
    }

    private static string BuildContent(string message, IReadOnlyDictionary<string, string>? metadata)
    {
        if (metadata is null || metadata.Count == 0)
            return message;

        var sb = new StringBuilder(message.TrimEnd());
        sb.AppendLine();
        sb.AppendLine();
        foreach (var kv in metadata)
            sb.AppendLine($"{kv.Key}: {kv.Value}");

        return sb.ToString();
    }
}
