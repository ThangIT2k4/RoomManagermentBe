using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class RoleEntity : AggregateRoot<Guid>
{
    public RoleCode Code { get; private set; }
    public RoleName Name { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RoleEntity()
    {
    }

    private RoleEntity(Guid id, RoleCode code, RoleName name)
    {
        Id = id;
        Code = code;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public static RoleEntity Create(Guid id, RoleCode code, RoleName name)
    {
        return new RoleEntity(id, code, name);
    }

    public void Rename(RoleName name)
    {
        Name = name;
    }
}
