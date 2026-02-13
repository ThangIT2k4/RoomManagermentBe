using System.Text.RegularExpressions;
using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class Phone : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        @"^(0|\+84)(3|5|7|8|9)\d{8}$",
        RegexOptions.Compiled
    );

    private const int MinLength = 10;
    private const int MaxLength = 12;

    private readonly string _value;

    private Phone(string value)
    {
        _value = value;
    }

    public static Phone Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new InvalidPhoneException(InvalidPhoneException.CodeEmpty);

        var normalized = Normalize(phone);

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new InvalidPhoneException(InvalidPhoneException.CodeInvalidLength);

        if (!PhoneRegex.IsMatch(normalized))
            throw new InvalidPhoneException(InvalidPhoneException.CodeInvalid);

        return new Phone(normalized);
    }

    private static string Normalize(string phone)
    {
        return new string(phone.Where(char.IsDigit).ToArray());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return _value;
    }
}
