using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public record RolePermissionId(Guid RoleId, Guid PermissionId);

public sealed class RolePermissionEntity : Entity<RolePermissionId>
{
    public Guid RoleId => Id.RoleId;
    public Guid PermissionId => Id.PermissionId;

    private RolePermissionEntity()
    {
    }

    private RolePermissionEntity(RolePermissionId id)
    {
        if (id.RoleId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.RoleIdEmpty);
        if (id.PermissionId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.PermissionIdEmpty);

        Id = id;
    }

    public static RolePermissionEntity Create(Guid roleId, Guid permissionId)
    {
        return new RolePermissionEntity(new RolePermissionId(roleId, permissionId));
    }
}
