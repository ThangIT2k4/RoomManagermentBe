using Integration.Domain.Enums;

namespace Integration.Domain.Entities;

public sealed class IntegrationConnectionEntity
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; }
    public string UserId { get; private set; }
    public IntegrationProvider Provider { get; private set; }
    public IntegrationConnectionStatus Status { get; private set; }
    public string ExternalAccountId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? LastSyncedAtUtc { get; private set; }

    private IntegrationConnectionEntity()
    {
        TenantId = string.Empty;
        UserId = string.Empty;
        ExternalAccountId = string.Empty;
    }

    private IntegrationConnectionEntity(
        Guid id,
        string tenantId,
        string userId,
        IntegrationProvider provider,
        string externalAccountId)
    {
        Id = id;
        TenantId = tenantId;
        UserId = userId;
        Provider = provider;
        Status = IntegrationConnectionStatus.Connected;
        ExternalAccountId = externalAccountId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static IntegrationConnectionEntity Create(
        string tenantId,
        string userId,
        IntegrationProvider provider,
        string externalAccountId)
    {
        return new IntegrationConnectionEntity(Guid.NewGuid(), tenantId, userId, provider, externalAccountId);
    }

    public void MarkDisconnected()
    {
        Status = IntegrationConnectionStatus.Disconnected;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = IntegrationConnectionStatus.Failed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkSynced()
    {
        LastSyncedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
