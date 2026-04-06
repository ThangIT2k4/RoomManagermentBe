namespace RoomManagerment.Messaging.Contracts.Events;

public record UserEmailVerifiedIntegrationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public DateTime VerifiedAt { get; init; }
    public string SourceService { get; init; } = default!;
}
