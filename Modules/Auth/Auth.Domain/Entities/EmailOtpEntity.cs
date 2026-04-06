using Auth.Domain.Common;
using Auth.Domain.Enums;
using Auth.Domain.Events;
using Auth.Domain.Exceptions;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class EmailOtpEntity : AggregateRoot<Guid>
{
    public Guid? UserId { get; private set; }
    public Email Email { get; private set; } = default!;
    public OtpCode OtpCode { get; private set; } = default!;
    public EmailOtpType Type { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private EmailOtpEntity() { }

    private EmailOtpEntity(Guid id, Guid? userId, Email email, OtpCode otpCode, EmailOtpType type, DateTimeOffset expiresAt, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        UserId = userId;
        Email = email;
        OtpCode = otpCode;
        Type = type;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static EmailOtpEntity Issue(Guid? userId, string email, string otpCode, EmailOtpType type, DateTimeOffset expiresAt, DateTime? createdAt = null)
    {
        var entity = new EmailOtpEntity(Guid.NewGuid(), userId, Email.Create(email), OtpCode.Create(otpCode), type, expiresAt, createdAt ?? DateTime.UtcNow, null);
        entity.AddDomainEvent(new EmailOtpIssuedEvent(entity.Id, entity.Email.Value, (int)entity.Type, DateTimeOffset.UtcNow));
        return entity;
    }

    public static EmailOtpEntity Reconstitute(Guid id, Guid? userId, string email, string otpCode, EmailOtpType type, DateTimeOffset expiresAt, DateTime? verifiedAt, bool isUsed, DateTime createdAt, DateTime? updatedAt)
        => new(id, userId, Email.Create(email), OtpCode.Create(otpCode), type, expiresAt, createdAt, updatedAt)
        {
            VerifiedAt = verifiedAt,
            IsUsed = isUsed
        };

    public bool CanResend(DateTimeOffset at, TimeSpan minInterval)
    {
        if (minInterval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(minInterval));
        }

        return at >= CreatedAt.Add(minInterval);
    }

    public bool IsMatch(string email, string otpCode, EmailOtpType type)
    {
        return Type == type
            && string.Equals(Email.Value, Email.Create(email).Value, StringComparison.OrdinalIgnoreCase)
            && string.Equals(OtpCode.Value, OtpCode.Create(otpCode).Value, StringComparison.Ordinal);
    }

    public void MarkVerified(DateTimeOffset verifiedAt)
    {
        EnsureNotExpired(verifiedAt);
        if (IsUsed)
        {
            throw new InvalidEmailOtpException(InvalidEmailOtpException.CodeUsed);
        }

        VerifiedAt = verifiedAt;
        IsUsed = true;
        UpdatedAt = verifiedAt.UtcDateTime;
        AddDomainEvent(new EmailOtpVerifiedEvent(Id, UserId, Email.Value, (int)Type, verifiedAt));
    }

    public void MarkUsed(DateTimeOffset usedAt)
    {
        if (IsUsed)
        {
            throw new InvalidEmailOtpException(InvalidEmailOtpException.CodeUsed);
        }

        EnsureNotExpired(usedAt);
        IsUsed = true;
        UpdatedAt = usedAt.UtcDateTime;
    }

    public bool IsExpired(DateTimeOffset at) => ExpiresAt <= at;

    private void EnsureNotExpired(DateTimeOffset at)
    {
        if (IsExpired(at))
        {
            throw new InvalidEmailOtpException(InvalidEmailOtpException.CodeExpired);
        }
    }
}

