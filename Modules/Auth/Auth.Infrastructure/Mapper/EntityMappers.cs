using System.Globalization;
using System.Net;
using Auth.Domain.Enums;
using DalSessionEntity = RoomManagerment.Auth.EntityClasses.SessionEntity;
using DalUserEntity = RoomManagerment.Auth.EntityClasses.UserEntity;
using DalAuditLogEntity = RoomManagerment.Auth.EntityClasses.AuditLogEntity;
using DalCapabilityEntity = RoomManagerment.Auth.EntityClasses.CapabilityEntity;
using DalEmailOtpEntity = RoomManagerment.Auth.EntityClasses.EmailOtpEntity;
using DalRoleEntity = RoomManagerment.Auth.EntityClasses.RoleEntity;
using DalUserProfileEntity = RoomManagerment.Auth.EntityClasses.UserProfileEntity;
using DomainSessionEntity = Auth.Domain.Entities.SessionEntity;
using DomainUserEntity = Auth.Domain.Entities.UserEntity;
using DomainAuditLogEntity = Auth.Domain.Entities.AuditLogEntity;
using DomainCapabilityEntity = Auth.Domain.Entities.CapabilityEntity;
using DomainEmailOtpEntity = Auth.Domain.Entities.EmailOtpEntity;
using DomainRoleEntity = Auth.Domain.Entities.RoleEntity;
using DomainUserProfileEntity = Auth.Domain.Entities.UserProfileEntity;

namespace Auth.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainUserEntity ToDomain(this DalUserEntity dal)
    {
        return DomainUserEntity.Reconstitute(
            dal.Id,
            dal.Email ?? string.Empty,
            dal.Username,
            dal.Status,
            dal.CreatedAt,
            dal.UpdatedAt,
            dal.Phone,
            dal.PasswordHash,
            dal.GoogleId,
            dal.RememberToken,
            dal.EmailVerifiedAt,
            dal.PhoneVerifiedAt,
            dal.LastLoginAt,
            dal.DeletedAt,
            dal.DeletedBy);
    }

    public static DalUserEntity ToPersistence(this DomainUserEntity domain)
    {
        return new DalUserEntity
        {
            Id = domain.Id,
            Email = domain.Email.Value,
            Username = domain.Username?.Value,
            Phone = domain.Phone?.Value,
            PasswordHash = domain.PasswordHash?.Value,
            GoogleId = domain.GoogleId,
            RememberToken = domain.RememberToken,
            Status = (short)domain.Status,
            EmailVerifiedAt = domain.EmailVerifiedAt,
            PhoneVerifiedAt = domain.PhoneVerifiedAt,
            LastLoginAt = domain.LastLoginAt,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            DeletedAt = domain.DeletedAt,
            DeletedBy = domain.DeletedBy
        };
    }

    /// <summary>
    /// Copies domain state onto a DAL entity that was loaded from the database so LLBLGen keeps "existing" entity state and issues UPDATE, not INSERT.
    /// </summary>
    public static void ApplyFromDomain(this DalUserEntity dal, DomainUserEntity domain)
    {
        dal.Email = domain.Email.Value;
        dal.Username = domain.Username?.Value;
        dal.Phone = domain.Phone?.Value;
        dal.PasswordHash = domain.PasswordHash?.Value;
        dal.GoogleId = domain.GoogleId;
        dal.RememberToken = domain.RememberToken;
        dal.Status = (short)domain.Status;
        dal.EmailVerifiedAt = domain.EmailVerifiedAt;
        dal.PhoneVerifiedAt = domain.PhoneVerifiedAt;
        dal.LastLoginAt = domain.LastLoginAt;
        dal.CreatedAt = domain.CreatedAt;
        dal.UpdatedAt = domain.UpdatedAt;
        dal.DeletedAt = domain.DeletedAt;
        dal.DeletedBy = domain.DeletedBy;
    }

    public static DomainSessionEntity ToDomain(this DalSessionEntity dal)
    {
        return DomainSessionEntity.Reconstitute(
            dal.Id,
            dal.UserId,
            dal.IpAddress?.ToString(),
            dal.UserAgent,
            dal.Payload,
            new DateTimeOffset(DateTime.SpecifyKind(dal.LastActivity, DateTimeKind.Utc)),
            dal.ExpiresAt.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(dal.ExpiresAt.Value, DateTimeKind.Utc)) : null,
            dal.CreatedAt);
    }

    public static DalSessionEntity ToPersistence(this DomainSessionEntity domain)
    {
        return new DalSessionEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            IpAddress = string.IsNullOrWhiteSpace(domain.IpAddress) ? null : IPAddress.TryParse(domain.IpAddress, out var ipAddress) ? ipAddress : null,
            UserAgent = domain.UserAgent,
            Payload = domain.PayloadJson,
            LastActivity = domain.LastActivity.UtcDateTime,
            ExpiresAt = domain.ExpiresAt?.UtcDateTime,
            CreatedAt = domain.CreatedAt
        };
    }

    public static void ApplyFromDomain(this DalSessionEntity dal, DomainSessionEntity domain)
    {
        dal.UserId = domain.UserId;
        dal.IpAddress = string.IsNullOrWhiteSpace(domain.IpAddress) ? null : IPAddress.TryParse(domain.IpAddress, out var ipAddress) ? ipAddress : null;
        dal.UserAgent = domain.UserAgent;
        dal.Payload = domain.PayloadJson;
        dal.LastActivity = domain.LastActivity.UtcDateTime;
        dal.ExpiresAt = domain.ExpiresAt?.UtcDateTime;
        dal.CreatedAt = domain.CreatedAt;
    }

    public static DomainUserProfileEntity ToDomain(this DalUserProfileEntity dal)
    {
        return DomainUserProfileEntity.Reconstitute(
            dal.UserId,
            NullIfEmpty(dal.FullName),
            NullIfEmpty(dal.Avatar),
            NullableDateOnly(dal.Dob),
            MapGender(dal.Gender),
            NullIfEmpty(dal.IdNumber),
            NullIfEmpty(dal.TaxCode),
            NullableDateOnly(dal.IdIssuedAt),
            NullIfEmpty(dal.IdCardPlace),
            NullIfEmpty(dal.IdImages),
            NullIfEmpty(dal.Address),
            NullIfEmpty(dal.Note),
            dal.SepayBankId,
            NullIfEmpty(dal.AccountNumber),
            NullIfEmpty(dal.AccountHolderName),
            NullIfEmpty(dal.BranchName),
            NullIfEmpty(dal.BranchCode),
            NullIfEmpty(dal.SwiftCode),
            NullIfEmpty(dal.BankingNotes),
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static DalUserProfileEntity ToPersistence(this DomainUserProfileEntity domain)
    {
        return new DalUserProfileEntity(domain.Id)
        {
            FullName = domain.FullName?.Value,
            Avatar = domain.Avatar,
            Dob = domain.Dob ?? default,
            Gender = domain.Gender is { } g ? (short)g : null,
            IdNumber = domain.IdNumber,
            TaxCode = domain.TaxCode,
            IdIssuedAt = domain.IdIssuedAt ?? default,
            IdCardPlace = domain.IdCardPlace,
            IdImages = domain.IdImagesJson,
            Address = domain.Address,
            Note = domain.Note,
            SepayBankId = domain.SepayBankId,
            AccountNumber = domain.AccountNumber,
            AccountHolderName = domain.AccountHolderName,
            BranchName = domain.BranchName,
            BranchCode = domain.BranchCode,
            SwiftCode = domain.SwiftCode,
            BankingNotes = domain.BankingNotes,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static void ApplyFromDomain(this DalUserProfileEntity dal, DomainUserProfileEntity domain)
    {
        dal.FullName = domain.FullName?.Value;
        dal.Avatar = domain.Avatar;
        dal.Dob = domain.Dob ?? default;
        dal.Gender = domain.Gender is { } g ? (short)g : null;
        dal.IdNumber = domain.IdNumber;
        dal.TaxCode = domain.TaxCode;
        dal.IdIssuedAt = domain.IdIssuedAt ?? default;
        dal.IdCardPlace = domain.IdCardPlace;
        dal.IdImages = domain.IdImagesJson;
        dal.Address = domain.Address;
        dal.Note = domain.Note;
        dal.SepayBankId = domain.SepayBankId;
        dal.AccountNumber = domain.AccountNumber;
        dal.AccountHolderName = domain.AccountHolderName;
        dal.BranchName = domain.BranchName;
        dal.BranchCode = domain.BranchCode;
        dal.SwiftCode = domain.SwiftCode;
        dal.BankingNotes = domain.BankingNotes;
        dal.CreatedAt = domain.CreatedAt;
        dal.UpdatedAt = domain.UpdatedAt;
    }

    public static DomainRoleEntity ToDomain(this DalRoleEntity dal)
    {
        return DomainRoleEntity.Reconstitute(
            dal.Id,
            dal.KeyCode ?? string.Empty,
            dal.Name ?? string.Empty,
            dal.Description,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static DalRoleEntity ToPersistence(this DomainRoleEntity domain)
    {
        return new DalRoleEntity(domain.Id)
        {
            KeyCode = domain.KeyCode.Value,
            Name = domain.Name,
            Description = domain.Description,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static void ApplyFromDomain(this DalRoleEntity dal, DomainRoleEntity domain)
    {
        dal.KeyCode = domain.KeyCode.Value;
        dal.Name = domain.Name;
        dal.Description = domain.Description;
        dal.CreatedAt = domain.CreatedAt;
        dal.UpdatedAt = domain.UpdatedAt;
    }

    public static DomainCapabilityEntity ToDomain(this DalCapabilityEntity dal)
    {
        return DomainCapabilityEntity.Reconstitute(
            dal.Id,
            dal.KeyCode ?? string.Empty,
            dal.Name ?? string.Empty,
            dal.Description,
            dal.Category,
            dal.DisplayOrder,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static DalCapabilityEntity ToPersistence(this DomainCapabilityEntity domain)
    {
        return new DalCapabilityEntity(domain.Id)
        {
            KeyCode = domain.KeyCode.Value,
            Name = domain.Name,
            Description = domain.Description,
            Category = domain.Category,
            DisplayOrder = domain.DisplayOrder,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static void ApplyFromDomain(this DalCapabilityEntity dal, DomainCapabilityEntity domain)
    {
        dal.KeyCode = domain.KeyCode.Value;
        dal.Name = domain.Name;
        dal.Description = domain.Description;
        dal.Category = domain.Category;
        dal.DisplayOrder = domain.DisplayOrder;
        dal.CreatedAt = domain.CreatedAt;
        dal.UpdatedAt = domain.UpdatedAt;
    }

    public static DomainEmailOtpEntity ToDomain(this DalEmailOtpEntity dal)
    {
        var type = ParseEmailOtpType(dal.Type ?? string.Empty);
        var verifiedAt = dal.VerifiedAt.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(dal.VerifiedAt.Value, DateTimeKind.Utc))
            : (DateTimeOffset?)null;

        return DomainEmailOtpEntity.Reconstitute(
            dal.Id,
            dal.UserId,
            dal.Email ?? string.Empty,
            dal.OtpCode ?? string.Empty,
            type,
            new DateTimeOffset(DateTime.SpecifyKind(dal.ExpiresAt, DateTimeKind.Utc)),
            verifiedAt.HasValue ? verifiedAt.Value.UtcDateTime : null,
            dal.IsUsed,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static DalEmailOtpEntity ToPersistence(this DomainEmailOtpEntity domain)
    {
        return new DalEmailOtpEntity(domain.Id)
        {
            UserId = domain.UserId,
            Email = domain.Email.Value,
            OtpCode = domain.OtpCode.Value,
            Type = EmailOtpTypeStorage.ToPersistedString(domain.Type),
            ExpiresAt = domain.ExpiresAt.UtcDateTime,
            IsUsed = domain.IsUsed,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            VerifiedAt = domain.VerifiedAt?.UtcDateTime
        };
    }

    public static void ApplyFromDomain(this DalEmailOtpEntity dal, DomainEmailOtpEntity domain)
    {
        dal.UserId = domain.UserId;
        dal.Email = domain.Email.Value;
        dal.OtpCode = domain.OtpCode.Value;
        dal.Type = EmailOtpTypeStorage.ToPersistedString(domain.Type);
        dal.ExpiresAt = domain.ExpiresAt.UtcDateTime;
        dal.IsUsed = domain.IsUsed;
        dal.CreatedAt = domain.CreatedAt;
        dal.UpdatedAt = domain.UpdatedAt;
        dal.VerifiedAt = domain.VerifiedAt?.UtcDateTime;
    }

    public static DomainAuditLogEntity ToDomain(this DalAuditLogEntity dal)
    {
        return DomainAuditLogEntity.Reconstitute(
            dal.Id,
            dal.ActorId,
            dal.OrganizationId,
            dal.Action ?? string.Empty,
            dal.EntityType ?? string.Empty,
            dal.EntityId,
            dal.BeforeJson,
            dal.AfterJson,
            dal.ChangesJson,
            dal.IpAddress?.ToString(),
            dal.UserAgent,
            dal.CreatedAt);
    }

    public static DalAuditLogEntity ToPersistence(this DomainAuditLogEntity domain)
    {
        return new DalAuditLogEntity(domain.Id)
        {
            ActorId = domain.ActorId,
            OrganizationId = domain.OrganizationId,
            Action = domain.Action,
            EntityType = domain.EntityType,
            EntityId = domain.EntityId,
            BeforeJson = domain.BeforeJson,
            AfterJson = domain.AfterJson,
            ChangesJson = domain.ChangesJson,
            IpAddress = string.IsNullOrWhiteSpace(domain.IpAddress)
                ? null
                : IPAddress.TryParse(domain.IpAddress, out var ip)
                    ? ip
                    : null,
            UserAgent = domain.UserAgent,
            CreatedAt = domain.CreatedAt
        };
    }

    private static string? NullIfEmpty(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var t = value.Trim();
        return t.Length == 0 ? null : t;
    }

    private static DateOnly? NullableDateOnly(DateOnly value) => value == default ? null : value;

    private static GenderType? MapGender(short? gender) => gender switch
    {
        null => null,
        (short)GenderType.Other => GenderType.Other,
        (short)GenderType.Male => GenderType.Male,
        (short)GenderType.Female => GenderType.Female,
        _ => GenderType.Other
    };

    private static EmailOtpType ParseEmailOtpType(string raw)
    {
        if (EmailOtpTypeStorage.TryParsePersisted(raw, out var fromStorage))
        {
            return fromStorage;
        }

        if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) &&
            Enum.IsDefined(typeof(EmailOtpType), n))
        {
            return (EmailOtpType)n;
        }

        if (Enum.TryParse(raw, true, out EmailOtpType byName))
        {
            return byName;
        }

        throw new InvalidOperationException($"Unknown email OTP type in storage: '{raw}'.");
    }
}

internal static class EmailOtpTypeStorage
{
    private static readonly Dictionary<string, EmailOtpType> PersistedToDomain =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["verify_email"] = EmailOtpType.VerifyEmail,
            ["reset_password"] = EmailOtpType.ResetPassword,
            ["login"] = EmailOtpType.Login,
            ["change_phone"] = EmailOtpType.ChangePhone,
            ["invite_user"] = EmailOtpType.InviteUser
        };

    public static string ToPersistedString(EmailOtpType type) => type switch
    {
        EmailOtpType.VerifyEmail => "verify_email",
        EmailOtpType.ResetPassword => "reset_password",
        EmailOtpType.Login => "login",
        EmailOtpType.ChangePhone => "change_phone",
        EmailOtpType.InviteUser => "invite_user",
        _ => ((int)type).ToString(CultureInfo.InvariantCulture)
    };

    public static bool TryParsePersisted(string raw, out EmailOtpType type)
    {
        if (PersistedToDomain.TryGetValue(raw.Trim(), out type))
        {
            return true;
        }

        type = default;
        return false;
    }
}
