using System.Net;
using DalSessionEntity = RoomManagerment.Auth.EntityClasses.SessionEntity;
using DalUserEntity = RoomManagerment.Auth.EntityClasses.UserEntity;
using DomainSessionEntity = Auth.Domain.Entities.SessionEntity;
using DomainUserEntity = Auth.Domain.Entities.UserEntity;

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
}
