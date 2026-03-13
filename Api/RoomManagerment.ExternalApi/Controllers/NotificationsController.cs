using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Messaging.Contracts.Events;

namespace RoomManagerment.ExternalApi.Controllers;

/// <summary>
/// Demo controller: publish NotificationCreateRequestedEvent qua RabbitMQ tới Notification API.
/// Trong thực tế, đây sẽ là các domain action (ví dụ: booking room, payment, etc.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationsController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new NotificationCreateRequestedEvent
        {
            RecipientUserId = request.RecipientUserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type ?? "Info",
            SourceService = "ExternalApi",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        }, cancellationToken);

        return Accepted(new { message = "Notification queued successfully" });
    }
}

public record SendNotificationRequest(
    Guid RecipientUserId,
    string Title,
    string Message,
    string? Type,
    Dictionary<string, string>? Metadata);
