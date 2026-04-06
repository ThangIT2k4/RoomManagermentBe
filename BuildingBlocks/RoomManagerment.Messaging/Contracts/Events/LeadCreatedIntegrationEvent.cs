namespace RoomManagerment.Messaging.Contracts.Events;

public record LeadCreatedIntegrationEvent
{
    public Guid LeadId { get; init; }
    public Guid OrganizationId { get; init; }
    public string? FullName { get; init; }
    public string Status { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public string SourceService { get; init; } = "CRM";
}
