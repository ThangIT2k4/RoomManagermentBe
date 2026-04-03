using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Identity.Domain.ValueObjects;

public sealed class Password : ValueObject
{
    private static readonly Regex StrongPasswordRegex = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
        RegexOptions.Compiled);

    public string Value { get; private set; }
    
    private Password(string value)
    {
        Value = value;
    }

    public static Password Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPasswordException(InvalidPasswordException.CodeEmpty);

        var password = value.Trim();

        if (password.Length < 8)
            throw new InvalidPasswordException(InvalidPasswordException.CodeTooShort);

        if (password.Length > 255)
            throw new InvalidPasswordException(InvalidPasswordException.CodeTooLong);

        if (password.Any(char.IsWhiteSpace))
            throw new InvalidPasswordException(InvalidPasswordException.CodeNoWhitespace);

        if (!StrongPasswordRegex.IsMatch(password))
            throw new InvalidPasswordException(InvalidPasswordException.CodeInvalid);

        return new Password(password);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}