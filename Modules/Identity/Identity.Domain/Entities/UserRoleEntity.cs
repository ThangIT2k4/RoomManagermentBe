using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public record UserRoleId(Guid UserId, Guid RoleId);

public sealed class UserRoleEntity : Entity<UserRoleId>
{
    public Guid UserId => Id.UserId;
    public Guid RoleId => Id.RoleId;

    private UserRoleEntity()
    {
    }

    private UserRoleEntity(UserRoleId id)
    {
        if (id.UserId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.UserIdEmpty);
        if (id.RoleId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.RoleIdEmpty);

        Id = id;
    }

    public static UserRoleEntity Create(Guid userId, Guid roleId)
    {
        return new UserRoleEntity(new UserRoleId(userId, roleId));
    }
}
