using Integration.Domain.Enums;

namespace Integration.Domain.Entities;

public sealed class IntegrationJobEntity
{
    public Guid Id { get; private set; }
    public Guid ConnectionId { get; private set; }
    public string ActionName { get; private set; }
    public string PayloadJson { get; private set; }
    public IntegrationJobStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }

    private IntegrationJobEntity()
    {
        ActionName = string.Empty;
        PayloadJson = "{}";
    }

    private IntegrationJobEntity(Guid connectionId, string actionName, string payloadJson)
    {
        Id = Guid.NewGuid();
        ConnectionId = connectionId;
        ActionName = actionName;
        PayloadJson = payloadJson;
        Status = IntegrationJobStatus.Queued;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static IntegrationJobEntity Create(Guid connectionId, string actionName, string payloadJson)
    {
        return new IntegrationJobEntity(connectionId, actionName, payloadJson);
    }

    public void MarkCompleted()
    {
        Status = IntegrationJobStatus.Completed;
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        Status = IntegrationJobStatus.Failed;
        LastError = error;
        RetryCount += 1;
        ProcessedAtUtc = DateTime.UtcNow;
    }
}
