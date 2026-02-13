using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    private const int MaxLength = 200;
    
    public string Value { get; private set; }
    
    private FullName(string value)
    {
        if (value?.Length > MaxLength == true)
            throw new InvalidUserProfileException(InvalidUserProfileException.FullNameTooLong);
        Value = value ?? string.Empty;
    }
    
    public static FullName Create(string? value) => new(value ?? string.Empty);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}