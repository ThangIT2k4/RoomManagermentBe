using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RoomManagerment.Gateway.Hubs;
using StackExchange.Redis;

namespace RoomManagerment.Gateway.Services;

/// <summary>
/// Subscribe Redis channel; khi Consumer (sau khi ghi DB Notification) publish message,
/// Gateway push realtime tới group user:{userId} qua SignalR.
/// Channel: RoomManagerment:Notification:Push
/// Message JSON: { "userId": "guid", "payload": { ... } }
/// </summary>
public class NotificationPushRedisSubscriber : BackgroundService
{
    private const string PushChannelName = "RoomManagerment:Notification:Push";
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly ILogger<NotificationPushRedisSubscriber> _logger;

    public NotificationPushRedisSubscriber(
        IConnectionMultiplexer redis,
        IHubContext<NotificationsHub> hubContext,
        ILogger<NotificationPushRedisSubscriber> logger)
    {
        _redis = redis;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var sub = _redis.GetSubscriber();
                await sub.SubscribeAsync(RedisChannel.Literal(PushChannelName), async (_, value) =>
                {
                    try
                    {
                        var json = value.ToString();
                        if (string.IsNullOrEmpty(json)) return;

                        var msg = JsonSerializer.Deserialize<PushMessage>(json);
                        if (msg is null || msg.UserId == Guid.Empty) return;

                        var groupName = NotificationsHub.GroupPrefix + msg.UserId.ToString();
                        await _hubContext.Clients.Group(groupName)
                            .SendAsync(NotificationsHub.EventNotificationCreated, msg.Payload ?? new object(), stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "NotificationPushRedisSubscriber: process message failed");
                    }
                });

                _logger.LogInformation("Subscribed Redis channel {Channel}", PushChannelName);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "NotificationPushRedisSubscriber: cannot subscribe to Redis channel {Channel}. Retrying in {DelaySeconds}s",
                    PushChannelName,
                    RetryDelay.TotalSeconds);

                try
                {
                    await Task.Delay(RetryDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    public sealed class PushMessage
    {
        public Guid UserId { get; set; }
        public object? Payload { get; set; }
    }
}
