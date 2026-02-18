using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class RefreshTokenEntity : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public TokenValue Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RefreshTokenEntity()
    {
    }

    private RefreshTokenEntity(Guid id, Guid userId, TokenValue token, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        CreatedAt = DateTime.UtcNow;
    }

    private RefreshTokenEntity(Guid id, Guid userId, TokenValue token, DateTime expiresAt, bool isRevoked, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = isRevoked;
        CreatedAt = createdAt;
    }

    public static RefreshTokenEntity Create(Guid id, Guid userId, TokenValue token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new InvalidRefreshTokenException(InvalidRefreshTokenException.UserIdEmpty);
        if (expiresAt <= DateTime.UtcNow)
            throw new InvalidRefreshTokenException(InvalidRefreshTokenException.ExpiresAtNotFuture);
        return new RefreshTokenEntity(id, userId, token, expiresAt);
    }

    public static RefreshTokenEntity Reconstitute(Guid id, Guid userId, TokenValue token, DateTime expiresAt, bool isRevoked, DateTime createdAt)
    {
        return new RefreshTokenEntity(id, userId, token, expiresAt, isRevoked, createdAt);
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsRevoked && !IsExpired;
}
