using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using RoomManagerment.Identity.EntityClasses;

namespace Identity.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static Identity.Domain.Entities.UserEntity ToDomain(this RoomManagerment.Identity.EntityClasses.UserEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.UserEntity.Reconstitute(
            dal.Id,
            Username.Create(dal.Username),
            Email.Create(dal.Email),
            PasswordHash.Create(dal.PasswordHash),
            (UserStatus)dal.Status,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static Identity.Domain.Entities.RoleEntity ToDomain(this RoomManagerment.Identity.EntityClasses.RoleEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.RoleEntity.Create(dal.Id, RoleCode.Create(dal.Code), RoleName.Create(dal.Name));
    }

    public static Identity.Domain.Entities.PermissionEntity ToDomain(this RoomManagerment.Identity.EntityClasses.PermissionEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.PermissionEntity.Create(dal.Id, PermissionCode.Create(dal.Code), dal.Name);
    }

    public static Identity.Domain.Entities.MenuEntity ToDomain(this RoomManagerment.Identity.EntityClasses.MenuEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.MenuEntity.Reconstitute(
            dal.Id,
            MenuCode.Create(dal.Code),
            MenuLabel.Create(dal.Label),
            dal.OrderIndex,
            string.IsNullOrWhiteSpace(dal.Path) ? null : MenuPath.Create(dal.Path),
            dal.Icon,
            dal.ParentId,
            dal.IsActive,
            dal.CreatedAt,
            dal.UpdatedAt);
    }

    public static Identity.Domain.Entities.RefreshTokenEntity ToDomain(this RoomManagerment.Identity.EntityClasses.RefreshTokenEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.RefreshTokenEntity.Reconstitute(
            dal.Id,
            dal.UserId,
            TokenValue.Create(dal.Token),
            dal.ExpiresAt,
            dal.IsRevoked,
            dal.CreatedAt);
    }

    public static Identity.Domain.Entities.MenuPermissionEntity ToDomain(this RoomManagerment.Identity.EntityClasses.MenuPermissionEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.MenuPermissionEntity.Create(dal.MenuId, PermissionCode.Create(dal.PermissionCode));
    }

    public static RoomManagerment.Identity.EntityClasses.MenuPermissionEntity ToDal(this Identity.Domain.Entities.MenuPermissionEntity domain)
    {
        if (domain is null) return null!;
        return new RoomManagerment.Identity.EntityClasses.MenuPermissionEntity(domain.Id.MenuId, domain.Id.PermissionCode.Value);
    }

    public static Identity.Domain.Entities.UserProfileEntity ToDomain(this RoomManagerment.Identity.EntityClasses.UserProfileEntity dal)
    {
        if (dal is null) return null!;
        FullName? fullName = string.IsNullOrWhiteSpace(dal.FullName) ? null : FullName.Create(dal.FullName);
        Phone? phone = null;
        if (!string.IsNullOrWhiteSpace(dal.Phone))
        {
            try { phone = Phone.Create(dal.Phone); } catch { /* ignore invalid phone */ }
        }
        return Identity.Domain.Entities.UserProfileEntity.Reconstitute(
            dal.UserId, fullName, phone, dal.AvatarUrl, dal.CreatedAt, dal.UpdatedAt);
    }

    public static Identity.Domain.Entities.UserRoleEntity ToDomain(this RoomManagerment.Identity.EntityClasses.UserRoleEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.UserRoleEntity.Create(dal.UserId, dal.RoleId);
    }

    public static Identity.Domain.Entities.RolePermissionEntity ToDomain(this RoomManagerment.Identity.EntityClasses.RolePermissionEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.RolePermissionEntity.Create(dal.RoleId, dal.PermissionId);
    }

    public static Identity.Domain.Entities.UserPermissionEntity ToDomain(this RoomManagerment.Identity.EntityClasses.UserPermissionEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.UserPermissionEntity.Create(dal.UserId, dal.PermissionId, dal.IsGranted);
    }

    public static Identity.Domain.Entities.UserMenuOverrideEntity ToDomain(this RoomManagerment.Identity.EntityClasses.UserMenuOverrideEntity dal)
    {
        if (dal is null) return null!;
        return Identity.Domain.Entities.UserMenuOverrideEntity.Create(dal.UserId, dal.MenuId, dal.IsVisible);
    }
}
