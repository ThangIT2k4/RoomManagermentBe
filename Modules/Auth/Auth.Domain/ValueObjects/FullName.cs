using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    private const int MaxLength = 200;

    public string Value { get; }

    private FullName(string value)
    {
        Value = value;
    }

    public static FullName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFullNameException(InvalidFullNameException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            throw new InvalidFullNameException(InvalidFullNameException.CodeTooLong);
        }

        return new FullName(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

