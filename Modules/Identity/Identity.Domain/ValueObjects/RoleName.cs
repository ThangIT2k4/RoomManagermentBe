using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class RoleName : ValueObject
{
    private const int MaxLength = 100;
    
    public string Value { get; private set; }
    
    private RoleName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidRoleException(InvalidRoleException.NameEmpty);
        if (value.Length > MaxLength)
            throw new InvalidRoleException(InvalidRoleException.NameTooLong);
        Value = value;
    }
    
    public static RoleName Create(string value) => new(value);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}