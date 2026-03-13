using Integration.Domain.Entities;
using Integration.Domain.Enums;

namespace Integration.Application.Dtos;

public sealed class IntegrationConnectionDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public IntegrationProvider Provider { get; set; }
    public IntegrationConnectionStatus Status { get; set; }
    public string ExternalAccountId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }

    public static IntegrationConnectionDto FromEntity(IntegrationConnectionEntity entity)
    {
        return new IntegrationConnectionDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            UserId = entity.UserId,
            Provider = entity.Provider,
            Status = entity.Status,
            ExternalAccountId = entity.ExternalAccountId,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc,
            LastSyncedAtUtc = entity.LastSyncedAtUtc
        };
    }
}
