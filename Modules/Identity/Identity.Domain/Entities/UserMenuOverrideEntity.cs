using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public record UserMenuOverrideId(Guid UserId, Guid MenuId);

public sealed class UserMenuOverrideEntity : Entity<UserMenuOverrideId>
{
    public Guid UserId => Id.UserId;
    public Guid MenuId => Id.MenuId;
    public bool IsVisible { get; private set; }

    private UserMenuOverrideEntity()
    {
    }

    private UserMenuOverrideEntity(UserMenuOverrideId id, bool isVisible)
    {
        if (id.UserId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.UserIdEmpty);
        if (id.MenuId == Guid.Empty)
            throw new InvalidEntityIdException(InvalidEntityIdException.MenuIdEmpty);

        Id = id;
        IsVisible = isVisible;
    }

    public static UserMenuOverrideEntity Create(Guid userId, Guid menuId, bool isVisible = true)
    {
        return new UserMenuOverrideEntity(new UserMenuOverrideId(userId, menuId), isVisible);
    }

    public void SetVisible(bool isVisible)
    {
        IsVisible = isVisible;
    }
}
