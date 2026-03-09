using Microsoft.AspNetCore.SignalR;
using RoomManagerment.Gateway.Transforms;

namespace RoomManagerment.Gateway.Hubs;

/// <summary>
/// SignalR Hub cho notification realtime.
/// Client kết nối với cookie session; userId lấy từ session, add vào group user:{userId}.
/// Consumer/MQ publish qua Redis channel, Gateway push vào group tương ứng.
/// </summary>
public class NotificationsHub : Hub
{
    public static string GroupPrefix => "user:";
    public static string EventNotificationCreated => "notification.created";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Session.GetString(SessionUserIdTransformProvider.SessionKeyUserId);
        if (string.IsNullOrEmpty(userId))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupPrefix + userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Session.GetString(SessionUserIdTransformProvider.SessionKeyUserId);
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupPrefix + userId);
        await base.OnDisconnectedAsync(exception);
    }
}
