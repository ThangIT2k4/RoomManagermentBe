using Auth.Domain.Common;
using Auth.Domain.Enums;
using Auth.Domain.Events;
using Auth.Domain.Exceptions;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class SessionEntity : AggregateRoot<string>
{
    public Guid UserId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? PayloadJson { get; private set; }
    public DateTimeOffset LastActivity { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private SessionEntity() { }

    private SessionEntity(string id, Guid userId, string? ipAddress, string? userAgent, string? payloadJson, DateTimeOffset lastActivity, DateTimeOffset? expiresAt, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        PayloadJson = payloadJson;
        LastActivity = lastActivity;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public static SessionEntity Create(Guid userId, string token, string? ipAddress = null, string? userAgent = null, string? payloadJson = null, DateTimeOffset? expiresAt = null, DateTime? createdAt = null)
    {
        var entity = new SessionEntity(SessionToken.Create(token).Value, userId, ipAddress?.Trim(), userAgent?.Trim(), payloadJson?.Trim(), DateTimeOffset.UtcNow, expiresAt, createdAt ?? DateTime.UtcNow);
        entity.AddDomainEvent(new SessionCreatedEvent(entity.Id, entity.UserId, DateTimeOffset.UtcNow));
        return entity;
    }

    public static SessionEntity Reconstitute(string id, Guid userId, string? ipAddress, string? userAgent, string? payloadJson, DateTimeOffset lastActivity, DateTimeOffset? expiresAt, DateTime createdAt, bool isRevoked = false)
        => new(SessionToken.Create(id).Value, userId, ipAddress, userAgent, payloadJson, lastActivity, expiresAt, createdAt) { IsRevoked = isRevoked };

    public void Refresh(DateTimeOffset activityAt, string? payloadJson = null)
    {
        if (IsExpired(activityAt))
        {
            throw new InvalidSessionException(InvalidSessionException.CodeExpired);
        }

        LastActivity = activityAt;
        PayloadJson = payloadJson?.Trim() ?? PayloadJson;
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        LastActivity = revokedAt;
        AddDomainEvent(new SessionRevokedEvent(Id, UserId, revokedAt));
    }

    public bool IsExpired(DateTimeOffset at)
    {
        return IsRevoked || (ExpiresAt.HasValue && ExpiresAt.Value <= at);
    }
}

