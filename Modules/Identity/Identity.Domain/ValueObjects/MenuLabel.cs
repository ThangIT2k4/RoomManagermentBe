using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class MenuLabel : ValueObject 
{
    private const int LabelMaxLength = 200;
    public string Value { get; private set; }

    private MenuLabel(string value)
    {
        Value = value;
    }

    public static MenuLabel Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidMenuException(InvalidMenuException.LabelEmpty);
        }
        if (value.Length > LabelMaxLength)
        {
            throw new InvalidMenuException(InvalidMenuException.LabelTooLong);
        }

        return new MenuLabel(value);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}