using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class RoleCode : ValueObject
{
    private const int MaxLength = 50;
    
    public string Value { get; private set; }
    
    private RoleCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidRoleException(InvalidRoleException.CodeEmpty);
        if (value.Length > MaxLength)
            throw new InvalidRoleException(InvalidRoleException.CodeTooLong);
        Value = value;
    }
    
    public static RoleCode Create(string value) => new(value);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}