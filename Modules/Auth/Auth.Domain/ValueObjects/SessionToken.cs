using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class SessionToken : ValueObject
{
    private const int MinLength = 16;
    private const int MaxLength = 128;

    public string Value { get; }

    private SessionToken(string value)
    {
        Value = value;
    }

    public static SessionToken Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSessionException(InvalidSessionException.CodeEmptyToken);
        }

        var trimmed = value.Trim();
        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
        {
            throw new InvalidSessionException(InvalidSessionException.CodeInvalidToken);
        }

        return new SessionToken(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

