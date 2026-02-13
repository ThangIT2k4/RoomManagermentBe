using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class PermissionCode : ValueObject
{
    private const int MaxLength = 50;
    
    public string Value { get; private set; }
    
    private PermissionCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPermissionException(InvalidPermissionException.CodeEmpty);
        if (value.Length > MaxLength)
            throw new InvalidPermissionException(InvalidPermissionException.CodeTooLong);
        Value = value;
    }
    
    public static PermissionCode Create(string value) => new(value);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}