using Identity.Domain.Common;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;
using Identity.Domain.Events;

namespace Identity.Domain.Entities;

public sealed class UserEntity : AggregateRoot<Guid>
{
    private const string TimestampMustBeUtcMessage = "Timestamp must be in UTC.";

    #region Properties
    public Username Username { get; private set; } 
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.InActive;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Constructors
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
    #endregion

    #region  Factory Methods (Domain Logic for Creation)

    public static UserEntity Create(Guid id, Username username, Email email, PasswordHash passwordHash)
    {
        return new UserEntity(id, username, email, passwordHash);
    }

    public static UserEntity Reconstitute(Guid id, Username username, Email email, PasswordHash passwordHash, UserStatus status, DateTime createdAt, DateTime? updatedAt)
    {
        return new UserEntity(id, username, email, passwordHash, status, createdAt, updatedAt);
    }

    #endregion

    #region Domain Comamnds (Business Logic - generate Domain Events)

    public void Activate(DateTime changeAt)
    {
        EnsureUtc(changeAt);

        if (Status == UserStatus.Active)
            return;

        if (Status == UserStatus.Locked)
            throw new InvalidUserStateException(InvalidUserStateException.CodeLockedToActivate);

        Status = UserStatus.Active;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserActivatedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void Deactivate(DateTime changeAt)
    {
        EnsureUtc(changeAt);

        if (Status == UserStatus.InActive)
            return;

        if (Status == UserStatus.Locked)
            throw new InvalidUserStateException(InvalidUserStateException.CodeInvalid);

        Status = UserStatus.InActive;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserDeactivatedEvent(Id, DateTimeOffset.UtcNow));
    }

    // Kept for backward compatibility with existing callers.
    public void Deactive(DateTime changeAt) => Deactivate(changeAt);
    
    public void Lock(DateTime changeAt)
    {
        EnsureUtc(changeAt);

        if (Status == UserStatus.Locked)
            return;

        Status = UserStatus.Locked;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserLockedEvent(Id, DateTimeOffset.UtcNow));
    }
    
    public void Unlock(DateTime changeAt)
    {
        EnsureUtc(changeAt);

        if (Status == UserStatus.Active)
            return;

        if (Status == UserStatus.InActive)
            throw new InvalidUserStateException(InvalidUserStateException.CodeInactiveToActivate);

        Status = UserStatus.Active;
        UpdatedAt = changeAt;
        AddDomainEvent(new UserUnlockedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        if (newPasswordHash is null)
            throw new InvalidPasswordHashException(InvalidPasswordHashException.CodeEmpty);

        if (PasswordHash == newPasswordHash)
            throw new InvalidPasswordHashException(InvalidPasswordHashException.CodeInvalid);

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserPasswordChangedEvent(Id, DateTimeOffset.UtcNow));
    }
    
    public void ResetPassword(PasswordHash newPasswordHash)
    {
        if (newPasswordHash is null)
            throw new InvalidPasswordHashException(InvalidPasswordHashException.CodeEmpty);

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserPasswordResetEvent(Id, DateTimeOffset.UtcNow));
    }

    public bool CanLogin()
    {
        return Status == UserStatus.Active;
    }

    private static void EnsureUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException(TimestampMustBeUtcMessage, nameof(value));
    }

    #endregion
    
}