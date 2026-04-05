using System.Text.RegularExpressions;
using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class Phone : ValueObject
{
    private const int MaxLength = 30;
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9]{7,30}$", RegexOptions.Compiled);

    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Phone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPhoneException(InvalidPhoneException.CodeEmpty);
        }

        var normalized = value.Trim().Replace(" ", string.Empty).Replace("-", string.Empty);
        if (normalized.Length > MaxLength)
        {
            throw new InvalidPhoneException(InvalidPhoneException.CodeTooLong);
        }

        if (!PhoneRegex.IsMatch(normalized))
        {
            throw new InvalidPhoneException(InvalidPhoneException.CodeInvalid);
        }

        return new Phone(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

