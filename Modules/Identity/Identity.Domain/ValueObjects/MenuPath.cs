using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class MenuPath : ValueObject 
{
    private const int PathMaxLength = 300;

    public string Value { get; private set; }

    private MenuPath(string value)
    {
        Value = value;
    }
    
    
    public static MenuPath Create(string value)
    {
        value = value.Trim();
        if (value.Length > PathMaxLength)
        {
            throw new InvalidMenuException(InvalidMenuException.PathTooLong);
        }

        return new MenuPath(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}