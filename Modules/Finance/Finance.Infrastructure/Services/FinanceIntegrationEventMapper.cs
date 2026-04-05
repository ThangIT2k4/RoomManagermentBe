using Finance.Domain.Events;
using RoomManagerment.Messaging.Contracts.Events;

namespace Finance.Infrastructure.Services;

internal static class FinanceIntegrationEventMapper
{
    private const string ExchangeName = "finance.events";

    public static bool TryMap(Finance.Domain.Common.IDomainEvent domainEvent, out IntegrationEventEnvelope? envelope)
    {
        envelope = domainEvent switch
        {
            NotificationRequestedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(NotificationCreateRequestedEvent),
                new NotificationCreateRequestedEvent
                {
                    RecipientUserId = e.RecipientUserId,
                    Title = e.Title,
                    Message = e.Message,
                    Type = e.Type,
                    SourceService = "Finance",
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    Metadata = e.Metadata is null ? null : new Dictionary<string, string>(e.Metadata)
                }),

            _ => null
        };

        return envelope is not null;
    }
}
