namespace RoomManagerment.Messaging.Contracts.Events;

public record UserLoggedInEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = default!;
    public string IpAddress { get; init; } = default!;
    public DateTime LoggedInAt { get; init; }
}
