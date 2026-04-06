using Auth.Domain.Common;
using Auth.Domain.Enums;
using Auth.Domain.Events;
using Auth.Domain.Exceptions;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class UserEntity : AggregateRoot<Guid>
{
    public Email Email { get; private set; } = default!;
    public Username? Username { get; private set; }
    public Phone? Phone { get; private set; }
    public PasswordHash? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public string? RememberToken { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }
    public DateTime? PhoneVerifiedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private UserEntity() { }

    private UserEntity(
        Guid id,
        Email email,
        Username? username,
        Phone? phone,
        PasswordHash? passwordHash,
        UserStatus status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        Email = email;
        Username = username;
        Phone = phone;
        PasswordHash = passwordHash;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserEntity Create(
        string email,
        string? username = null,
        string? phone = null,
        string? passwordHash = null,
        short status = (short)UserStatus.Active,
        DateTime? createdAt = null)
        => Create(
            Email.Create(email),
            username is null ? null : Username.Create(username),
            phone is null ? null : Phone.Create(phone),
            passwordHash is null ? null : PasswordHash.Create(passwordHash),
            status,
            createdAt);

    public static UserEntity RegisterByEmail(
        string email,
        string? username = null,
        string? phone = null,
        string? passwordHash = null,
        DateTime? createdAt = null)
        => Create(email, username, phone, passwordHash, (short)UserStatus.Inactive, createdAt);

    public static UserEntity CreateGoogleAccount(
        string email,
        string googleId,
        string? username = null,
        string? phone = null,
        DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(googleId))
        {
            throw new ArgumentException("Google id cannot be empty.", nameof(googleId));
        }

        var entity = Create(email, username, phone, null, (short)UserStatus.Active, createdAt);
        var now = entity.CreatedAt;
        entity.GoogleId = googleId.Trim();
        entity.EmailVerifiedAt = now;
        entity.UpdatedAt = now;
        return entity;
    }

    public static UserEntity Create(
        Email email,
        Username? username = null,
        Phone? phone = null,
        PasswordHash? passwordHash = null,
        short status = (short)UserStatus.Active,
        DateTime? createdAt = null)
    {
        var userStatus = ToUserStatus(status);
        var entity = new UserEntity(
            Guid.NewGuid(),
            email,
            username,
            phone,
            passwordHash,
            userStatus,
            createdAt ?? DateTime.UtcNow,
            null);

        entity.AddDomainEvent(new UserCreatedEvent(
            entity.Id,
            entity.Email.Value,
            entity.Username?.Value,
            entity.Phone?.Value,
            (short)entity.Status,
            DateTimeOffset.UtcNow));

        return entity;
    }

    public static UserEntity Reconstitute(
        Guid id,
        string email,
        string? username,
        short status,
        DateTime createdAt,
        DateTime? updatedAt,
        string? phone = null,
        string? passwordHash = null,
        string? googleId = null,
        string? rememberToken = null,
        DateTime? emailVerifiedAt = null,
        DateTime? phoneVerifiedAt = null,
        DateTime? lastLoginAt = null,
        DateTime? deletedAt = null,
        Guid? deletedBy = null)
    {
        var entity = new UserEntity(
            id,
            Email.Create(email),
            username is null ? null : Username.Create(username),
            phone is null ? null : Phone.Create(phone),
            passwordHash is null ? null : PasswordHash.Create(passwordHash),
            ToUserStatus(status),
            createdAt,
            updatedAt)
        {
            GoogleId = googleId,
            RememberToken = rememberToken,
            EmailVerifiedAt = emailVerifiedAt,
            PhoneVerifiedAt = phoneVerifiedAt,
            LastLoginAt = lastLoginAt,
            DeletedAt = deletedAt,
            DeletedBy = deletedBy
        };

        return entity;
    }

    public static UserEntity FromPersistence(
        Guid id,
        string email,
        string? username,
        short status,
        DateTime createdAt,
        DateTime? updatedAt)
        => Reconstitute(id, email, username, status, createdAt, updatedAt);

    public void ChangeEmail(Email email, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (Email == email)
        {
            return;
        }

        Email = email;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserEmailChangedEvent(Id, email.Value, DateTimeOffset.UtcNow));
    }

    public void ChangeUsername(Username? username, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (Username == username)
        {
            return;
        }

        Username = username;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserUsernameChangedEvent(Id, username?.Value, DateTimeOffset.UtcNow));
    }

    public void ChangePhone(Phone? phone, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (Phone == phone)
        {
            return;
        }

        Phone = phone;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserPhoneChangedEvent(Id, phone?.Value, DateTimeOffset.UtcNow));
    }

    public void SetPassword(PasswordHash passwordHash, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (PasswordHash == passwordHash)
        {
            return;
        }

        PasswordHash = passwordHash;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserPasswordChangedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void MarkEmailVerified(DateTime verifiedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(verifiedAt);

        EmailVerifiedAt = verifiedAt;
        UpdatedAt = verifiedAt;
        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email.Value, DateTimeOffset.UtcNow));
    }

    public void MarkPhoneVerified(DateTime verifiedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(verifiedAt);

        PhoneVerifiedAt = verifiedAt;
        UpdatedAt = verifiedAt;
        AddDomainEvent(new UserPhoneVerifiedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void RecordLogin(DateTime loginAt, string? rememberToken = null, string? ipAddress = null)
    {
        EnsureNotDeleted();
        EnsureUtc(loginAt);

        if (Status != UserStatus.Active)
        {
            throw new InvalidUserStateException(Status == UserStatus.Banned
                ? InvalidUserStateException.CodeBannedUser
                : InvalidUserStateException.CodeInactiveUser);
        }

        LastLoginAt = loginAt;
        RememberToken = rememberToken;
        UpdatedAt = loginAt;
        AddDomainEvent(new UserLoginRecordedEvent(Id, Username?.Value ?? Email.Value, ipAddress, DateTimeOffset.UtcNow));
    }

    public void Activate(DateTime changedAt)
    {
        ChangeStatus(UserStatus.Active, changedAt);
    }

    public void Deactivate(DateTime changedAt)
    { 
        ChangeStatus(UserStatus.Inactive, changedAt);
    }

    public void Ban(DateTime changedAt)
    {
        ChangeStatus(UserStatus.Banned, changedAt);
    }

    public bool CanLogin() => Status == UserStatus.Active && DeletedAt is null;

    public void VerifyEmailAndActivate(DateTime verifiedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(verifiedAt);

        var previousStatus = Status;
        EmailVerifiedAt = verifiedAt;
        if (Status == UserStatus.Inactive)
        {
            Status = UserStatus.Active;
        }

        UpdatedAt = verifiedAt;
        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email.Value, DateTimeOffset.UtcNow));
        if (previousStatus != Status)
        {
            AddDomainEvent(new UserStatusChangedEvent(Id, (short)previousStatus, (short)Status, DateTimeOffset.UtcNow));
        }
    }

    public void LinkGoogleAccount(string googleId, DateTime linkedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(linkedAt);

        if (string.IsNullOrWhiteSpace(googleId))
        {
            throw new ArgumentException("Google id cannot be empty.", nameof(googleId));
        }

        var normalized = googleId.Trim();
        if (GoogleId == normalized)
        {
            return;
        }

        GoogleId = normalized;
        if (EmailVerifiedAt is null)
        {
            EmailVerifiedAt = linkedAt;
        }

        UpdatedAt = linkedAt;
    }

    public void SetRememberToken(string rememberToken, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (string.IsNullOrWhiteSpace(rememberToken))
        {
            throw new ArgumentException("Remember token cannot be empty.", nameof(rememberToken));
        }

        var normalized = rememberToken.Trim();
        if (RememberToken == normalized)
        {
            return;
        }

        RememberToken = normalized;
        UpdatedAt = changedAt;
    }

    public void ClearRememberToken(DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (RememberToken is null)
        {
            return;
        }

        RememberToken = null;
        UpdatedAt = changedAt;
    }

    public void SoftDelete(Guid deletedBy, DateTime deletedAt)
    {
        EnsureUtc(deletedAt);

        if (deletedBy == Guid.Empty)
        {
            throw new ArgumentException("DeletedBy must be a valid identifier.", nameof(deletedBy));
        }

        if (DeletedAt.HasValue)
        {
            return;
        }

        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        UpdatedAt = deletedAt;
    }

    private void ChangeStatus(UserStatus targetStatus, DateTime changedAt)
    {
        EnsureNotDeleted();
        EnsureUtc(changedAt);

        if (Status == targetStatus)
        {
            return;
        }

        var fromStatus = Status;
        Status = targetStatus;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserStatusChangedEvent(Id, (short)fromStatus, (short)targetStatus, DateTimeOffset.UtcNow));
    }

    private static UserStatus ToUserStatus(short status)
    {
        if (!Enum.IsDefined(typeof(UserStatus), status))
        {
            throw new InvalidUserStateException(InvalidUserStateException.CodeInvalidStatus);
        }

        return (UserStatus)status;
    }

    private static void EnsureUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Timestamp must be in UTC.", nameof(value));
        }
    }

    private void EnsureNotDeleted()
    {
        if (DeletedAt is not null)
        {
            throw new InvalidUserStateException(InvalidUserStateException.CodeInactiveUser);
        }
    }
}

