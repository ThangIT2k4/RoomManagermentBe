using System.Text.RegularExpressions;
using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private const int MaxLength = 200;
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value.Trim().ToLowerInvariant();
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEmailException(InvalidEmailException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            throw new InvalidEmailException(InvalidEmailException.CodeTooLong);
        }

        if (!EmailRegex.IsMatch(trimmed))
        {
            throw new InvalidEmailException(InvalidEmailException.CodeInvalid);
        }

        return new Email(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

