namespace RoomManagerment.Messaging.Contracts.Events;

public record SessionRevokedIntegrationEvent
{
    public string SessionId { get; init; } = default!;
    public Guid UserId { get; init; }
    public DateTime RevokedAt { get; init; }
    public string SourceService { get; init; } = "Auth";
}

