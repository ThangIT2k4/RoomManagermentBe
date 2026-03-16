namespace RoomManagerment.Messaging.Contracts.Events;
public record NotificationCreateRequestedEvent
{
    public Guid RecipientUserId { get; init; }
    public string Title { get; init; } = default!;
    public string Message { get; init; } = default!;
    public string Type { get; init; } = "Info";
    public string SourceService { get; init; } = default!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public Dictionary<string, string>? Metadata { get; init; }
}
