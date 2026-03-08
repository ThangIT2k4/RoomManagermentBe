namespace Notification.Domain.Entities;

public sealed class NotificationEntity
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Type { get; init; }
    public DateTime CreatedAt { get; init; }
}
