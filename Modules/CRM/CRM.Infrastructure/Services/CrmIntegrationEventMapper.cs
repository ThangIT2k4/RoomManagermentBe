using CRM.Domain.Common;
using CRM.Domain.Events;
using RoomManagerment.Messaging.Contracts.Events;

namespace CRM.Infrastructure.Services;

internal static class CrmIntegrationEventMapper
{
    private const string ExchangeName = "crm.events";

    public static bool TryMap(IDomainEvent domainEvent, out IntegrationEventEnvelope? envelope)
    {
        envelope = domainEvent switch
        {
            LeadCreatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeadCreatedIntegrationEvent),
                new LeadCreatedIntegrationEvent
                {
                    LeadId = e.LeadId,
                    OrganizationId = e.OrganizationId,
                    FullName = e.FullName,
                    Status = e.Status,
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "CRM"
                }),

            LeadStatusChangedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeadStatusChangedIntegrationEvent),
                new LeadStatusChangedIntegrationEvent
                {
                    LeadId = e.LeadId,
                    PreviousStatus = e.PreviousStatus,
                    CurrentStatus = e.CurrentStatus,
                    ChangedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "CRM"
                }),

            _ => null
        };

        return envelope is not null;
    }
}
