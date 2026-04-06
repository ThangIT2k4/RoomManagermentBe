namespace CRM.Infrastructure.Services;

internal sealed record IntegrationEventEnvelope(
    string Exchange,
    string RoutingKey,
    object Message);
