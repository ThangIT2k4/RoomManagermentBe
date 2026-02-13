using System.Text.RegularExpressions;
using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private const int MaxLength = 200;
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private readonly string _value;

    public string Value => _value;

    private Email(string value)
    {
        _value = value.Trim().ToLowerInvariant();
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidEmailException(InvalidEmailException.CodeEmpty);

        var trimmed = email.Trim();
        if (trimmed.Length > MaxLength)
            throw new InvalidEmailException(InvalidEmailException.CodeTooLong);

        if (!EmailRegex.IsMatch(trimmed))
            throw new InvalidEmailException(InvalidEmailException.CodeInvalid);

        return new Email(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return _value;
    }
}
