namespace Organization.Infrastructure.Services;

internal sealed record IntegrationEventEnvelope(string RoutingKey, object Message);
