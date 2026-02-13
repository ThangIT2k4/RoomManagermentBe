using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public record UserPermissionId(Guid UserId, Guid PermissionId);

public sealed class UserPermissionEntity : Entity<UserPermissionId>
{
    public Guid UserId => Id.UserId;
    public Guid PermissionId => Id.PermissionId;
    public bool IsGranted { get; private set; }

    private UserPermissionEntity()
    {
    }

    private UserPermissionEntity(UserPermissionId id, bool isGranted)
    {
        if (id.UserId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.UserIdEmpty);
        if (id.PermissionId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.PermissionIdEmpty);

        Id = id;
        IsGranted = isGranted;
    }

    public static UserPermissionEntity Create(Guid userId, Guid permissionId, bool isGranted = true)
    {
        return new UserPermissionEntity(new UserPermissionId(userId, permissionId), isGranted);
    }

    public void SetGranted(bool isGranted)
    {
        IsGranted = isGranted;
    }
}
