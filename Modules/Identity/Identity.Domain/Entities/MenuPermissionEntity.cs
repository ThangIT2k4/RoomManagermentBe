using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public record MenuPermissionId(Guid MenuId, PermissionCode PermissionCode);

public sealed class MenuPermissionEntity : Entity<MenuPermissionId>
{
    private const int PermissionCodeMaxLength = 50;

    public Guid MenuId => Id.MenuId;
    public PermissionCode PermissionCode => Id.PermissionCode;

    private MenuPermissionEntity()
    {
    }

    private MenuPermissionEntity(MenuPermissionId id)
    {
        if (id.MenuId == Guid.Empty)
            throw new InvalidMenuPermissionException(InvalidMenuPermissionException.MenuIdEmpty);
        Id = id;
    }

    public static MenuPermissionEntity Create(Guid menuId, PermissionCode permissionCode)
    {
        return new MenuPermissionEntity(new MenuPermissionId(menuId, permissionCode));
    }
}
