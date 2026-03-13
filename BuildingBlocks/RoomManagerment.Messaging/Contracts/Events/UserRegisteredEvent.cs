namespace RoomManagerment.Messaging.Contracts.Events;

public record UserRegisteredEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public DateTime RegisteredAt { get; init; }
}
