namespace RoomManagerment.Messaging.Contracts.Events;

public record SessionCreatedIntegrationEvent
{
    public string SessionId { get; init; } = default!;
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string SourceService { get; init; } = "Auth";
}

