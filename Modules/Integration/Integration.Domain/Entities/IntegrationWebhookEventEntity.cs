using Integration.Domain.Enums;

namespace Integration.Domain.Entities;

public sealed class IntegrationWebhookEventEntity
{
    public Guid Id { get; private set; }
    public IntegrationProvider Provider { get; private set; }
    public string ExternalEventId { get; private set; }
    public string Signature { get; private set; }
    public string PayloadJson { get; private set; }
    public bool IsProcessed { get; private set; }
    public DateTime ReceivedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }

    private IntegrationWebhookEventEntity()
    {
        ExternalEventId = string.Empty;
        Signature = string.Empty;
        PayloadJson = "{}";
    }

    private IntegrationWebhookEventEntity(
        IntegrationProvider provider,
        string externalEventId,
        string signature,
        string payloadJson)
    {
        Id = Guid.NewGuid();
        Provider = provider;
        ExternalEventId = externalEventId;
        Signature = signature;
        PayloadJson = payloadJson;
        ReceivedAtUtc = DateTime.UtcNow;
    }

    public static IntegrationWebhookEventEntity Create(
        IntegrationProvider provider,
        string externalEventId,
        string signature,
        string payloadJson)
    {
        return new IntegrationWebhookEventEntity(provider, externalEventId, signature, payloadJson);
    }

    public void MarkProcessed()
    {
        IsProcessed = true;
        ProcessedAtUtc = DateTime.UtcNow;
    }
}
