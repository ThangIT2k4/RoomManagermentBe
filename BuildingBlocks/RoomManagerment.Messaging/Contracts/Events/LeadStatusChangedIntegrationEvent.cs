namespace RoomManagerment.Messaging.Contracts.Events;

public record LeadStatusChangedIntegrationEvent
{
    public Guid LeadId { get; init; }
    public string PreviousStatus { get; init; } = default!;
    public string CurrentStatus { get; init; } = default!;
    public DateTime ChangedAt { get; init; }
    public string SourceService { get; init; } = "CRM";
}
