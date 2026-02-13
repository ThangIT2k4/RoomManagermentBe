using Identity.Domain.Common;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;
using Identity.Domain.Events;

namespace Identity.Domain.Entities;

public sealed class UserEntity : AggregateRoot<Guid>
{
    public Username Username { get; private set; } 
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.InActive;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private UserEntity() { } 

    private UserEntity(Guid id, Username username, Email email, PasswordHash passwordHash)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        Status = UserStatus.InActive;
        AddDomainEvent(new UserCreatedEvent(
            id,
            username.Value,
            email.Value,
            DateTimeOffset.UtcNow));
    }
    
    private UserEntity(Guid id, Username username, Email email, PasswordHash passwordHash, UserStatus status, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserEntity Create(Guid id, Username username, Email email, PasswordHash passwordHash)
    {
        return new UserEntity(id, username, email, passwordHash);
    }

    public static UserEntity Reconstitute(Guid id, Username username, Email email, PasswordHash passwordHash, UserStatus status, DateTime createdAt, DateTime? updatedAt)
    {
        return new UserEntity(id, username, email, passwordHash, status, createdAt, updatedAt);
    }

    public void Activate(DateTime changeAt)
    {
        if (Status == UserStatus.Locked)
            throw new InvalidUserStateException(InvalidUserStateException.CodeLockedToActivate);
        Status = UserStatus.Active;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserActivatedEvent(
            Id,
            DateTimeOffset.UtcNow));
    }

    public void Deactive(DateTime changeAt)
    {
        if (Status == UserStatus.Locked)
            throw new InvalidUserStateException(InvalidUserStateException.CodeInactiveToActivate);
        Status = UserStatus.InActive;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserDeactivatedEvent(
            Id,
            DateTimeOffset.UtcNow));
    }
    
    public void Lock(DateTime changeAt)
    {
        if (Status == UserStatus.Locked)
            return;
        Status = UserStatus.Locked;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserLockedEvent(
            Id,
            DateTimeOffset.UtcNow));
    }
    
    public void Unlock(DateTime changeAt)
    {
        if (Status == UserStatus.Active)
            return;
        Status = UserStatus.Active;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserUnlockedEvent(
            Id,
            DateTimeOffset.UtcNow));
    }
}