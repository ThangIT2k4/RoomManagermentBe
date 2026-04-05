using System.Text.RegularExpressions;
using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class Username : ValueObject
{
    private const int MaxLength = 50;
    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{2,50}$", RegexOptions.Compiled);

    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidUsernameException(InvalidUsernameException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            throw new InvalidUsernameException(InvalidUsernameException.CodeTooLong);
        }

        if (!UsernameRegex.IsMatch(trimmed))
        {
            throw new InvalidUsernameException(InvalidUsernameException.CodeInvalid);
        }

        return new Username(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

