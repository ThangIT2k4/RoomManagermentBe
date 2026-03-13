namespace RoomManagerment.Messaging.Contracts.Events;

/// <summary>
/// Published bởi bất kỳ service nào khi cần gửi notification tới user.
/// Consumer: Notification API (lưu vào DB + push realtime qua SignalR)
/// Exchange: rm.notification (direct)
/// Queue: notification.create
/// </summary>
public record NotificationCreateRequestedEvent
{
    public Guid RecipientUserId { get; init; }
    public string Title { get; init; } = default!;
    public string Message { get; init; } = default!;

    /// <summary>Loại notification: Info, Warning, Success, Error</summary>
    public string Type { get; init; } = "Info";

    /// <summary>Service nguồn phát ra event</summary>
    public string SourceService { get; init; } = default!;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Metadata tuỳ chọn (link, entity id, v.v.)</summary>
    public Dictionary<string, string>? Metadata { get; init; }
}
