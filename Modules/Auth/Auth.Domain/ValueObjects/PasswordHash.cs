using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    private const int MaxLength = 500;

    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPasswordHashException(InvalidPasswordHashException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            throw new InvalidPasswordHashException(InvalidPasswordHashException.CodeTooLong);
        }

        return new PasswordHash(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

