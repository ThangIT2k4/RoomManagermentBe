namespace RoomManagerment.Messaging.Contracts.Events;

public record PasswordChangedEvent
{
    public Guid UserId { get; init; }
    public DateTime ChangedAt { get; init; }
    public string SourceService { get; init; } = "Identity";
}