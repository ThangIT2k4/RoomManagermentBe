using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class OtpCode : ValueObject
{
    private const int MinLength = 4;
    private const int MaxLength = 20;

    public string Value { get; }

    private OtpCode(string value)
    {
        Value = value;
    }

    public static OtpCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEmailOtpException(InvalidEmailOtpException.CodeEmptyOtp);
        }

        var trimmed = value.Trim();
        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
        {
            throw new InvalidEmailOtpException(InvalidEmailOtpException.CodeInvalidOtp);
        }

        return new OtpCode(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

