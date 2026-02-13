using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class PermissionEntity : AggregateRoot<Guid>
{
    private const int NameMaxLength = 100;

    public PermissionCode Code { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private PermissionEntity()
    {
    }

    private PermissionEntity(Guid id, PermissionCode code, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidPermissionException(InvalidPermissionException.NameEmpty);
        if (name.Length > NameMaxLength)
            throw new InvalidPermissionException(InvalidPermissionException.NameTooLong);

        Id = id;
        Code = code;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public static PermissionEntity Create(Guid id, PermissionCode code, string name)
    {
        return new PermissionEntity(id, code, name);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidPermissionException(InvalidPermissionException.NameEmpty);
        if (name.Length > NameMaxLength)
            throw new InvalidPermissionException(InvalidPermissionException.NameTooLong);
        Name = name;
    }
}
