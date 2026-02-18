using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class UserProfileEntity : Entity<Guid>
{
    public Guid UserId => Id;
    public FullName? FullName { get; private set; }
    public Phone ? Phone { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private UserProfileEntity()
    {
    }

    private UserProfileEntity(Guid userId, FullName? fullName, Phone? phone, string? avatarUrl)
    {
        if (userId == Guid.Empty)
            throw new InvalidUserProfileException(InvalidUserProfileException.UserIdEmpty);
        
        Id = userId;
        FullName = fullName;
        Phone = phone;
        AvatarUrl = avatarUrl;
        CreatedAt = DateTime.UtcNow;
    }

    private UserProfileEntity(Guid userId, FullName? fullName, Phone? phone, string? avatarUrl, DateTime createdAt, DateTime? updatedAt)
    {
        Id = userId;
        FullName = fullName;
        Phone = phone;
        AvatarUrl = avatarUrl;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserProfileEntity Create(Guid userId, FullName? fullName = null, Phone? phone = null, string? avatarUrl = null)
    {
        return new UserProfileEntity(userId, fullName, phone, avatarUrl, DateTime.UtcNow, null);
    }

    public static UserProfileEntity Reconstitute(Guid userId, FullName? fullName, Phone? phone, string? avatarUrl, DateTime createdAt, DateTime? updatedAt)
    {
        return new UserProfileEntity(userId, fullName, phone, avatarUrl, createdAt, updatedAt);
    }

    public void Update(FullName? fullName, Phone? phone, string? avatarUrl, DateTime updatedAt)
    {
        FullName = fullName;
        Phone = phone;
        AvatarUrl = avatarUrl;
        UpdatedAt = updatedAt;
    }
}
