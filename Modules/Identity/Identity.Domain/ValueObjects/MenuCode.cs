using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class MenuCode : ValueObject
{
    private const int CodeMaxLength = 100;

    public string Value { get; private set; } 
    
    private MenuCode(string value)
    {
        Value = value;
    }

    public static MenuCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidMenuException(InvalidMenuException.CodeEmpty);
        }
        if (value.Length > CodeMaxLength)
        {
            throw new InvalidMenuException(InvalidMenuException.CodeTooLong);
        }

        return new MenuCode(value);
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}