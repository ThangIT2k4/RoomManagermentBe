using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;

namespace CRM.Domain.Entities;

public sealed class ViewingEntity
{
    public Guid Id { get; private set; }
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
            throw new DomainValidationException("OrganizationId is required.");
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
}
