using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;
using CRM.Domain.Events;

namespace CRM.Domain.Entities;

public sealed class ViewingEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid? AgentId { get; private set; }
    public DateTime ScheduleAt { get; private set; }
    public string Status { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ViewingEntity(
        Guid id,
        Guid organizationId,
        Guid? leadId,
        Guid? agentId,
        DateTime scheduleAt,
        string status,
        string? note,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        LeadId = leadId;
        AgentId = agentId;
        ScheduleAt = scheduleAt;
        Status = status;
        Note = note;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static ViewingEntity Create(
        Guid organizationId,
        DateTime scheduleAt,
        Guid? leadId = null,
        Guid? agentId = null,
        string status = "scheduled",
        string? note = null,
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId là bắt buộc.");
        }

        return new ViewingEntity(
            Guid.NewGuid(),
            organizationId,
            leadId,
            agentId,
            scheduleAt,
            EnumValueParser.ParseRequired<ViewingStatus>(status, nameof(status)),
            note?.Trim(),
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static ViewingEntity Reconstitute(
        Guid id,
        Guid organizationId,
        Guid? leadId,
        Guid? agentId,
        DateTime scheduleAt,
        string status,
        string? note,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new ViewingEntity(id, organizationId, leadId, agentId, scheduleAt, status, note, createdAt, updatedAt);
    }

    public void Confirm(DateTime? updatedAt = null)
    {
        EnsureTransition("scheduled", "confirmed");
        Status = "confirmed";
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ViewingStatusChangedEvent(Id, "scheduled", Status, DateTimeOffset.UtcNow));
    }

    public void Complete(string? resultNote = null, DateTime? updatedAt = null)
    {
        var normalized = NormalizeStatus(Status);
        if (normalized is not ("scheduled" or "confirmed"))
        {
            throw new DomainValidationException("Lịch xem chỉ có thể hoàn tất từ trạng thái scheduled/confirmed.");
        }

        Status = ViewingStatus.Completed.ToString().ToLowerInvariant();
        Note = string.IsNullOrWhiteSpace(resultNote) ? Note : resultNote.Trim();
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ViewingStatusChangedEvent(Id, normalized, Status, DateTimeOffset.UtcNow));
    }

    public void Cancel(string? reason = null, DateTime? updatedAt = null)
    {
        var normalized = NormalizeStatus(Status);
        if (normalized is not ("scheduled" or "confirmed"))
        {
            throw new DomainValidationException("Lịch xem chỉ có thể hủy từ trạng thái scheduled/confirmed.");
        }

        Status = ViewingStatus.Cancelled.ToString().ToLowerInvariant();
        Note = string.IsNullOrWhiteSpace(reason) ? Note : reason.Trim();
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ViewingStatusChangedEvent(Id, normalized, Status, DateTimeOffset.UtcNow));
    }

    private void EnsureTransition(string expected, string next)
    {
        var current = NormalizeStatus(Status);
        if (!string.Equals(current, expected, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainValidationException($"Chuyển trạng thái lịch xem không hợp lệ từ '{current}' sang '{next}'.");
        }
    }

    private static string NormalizeStatus(string value) => value.Trim().ToLowerInvariant();
}
