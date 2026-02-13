using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class TokenValue : ValueObject
{
    private const int MaxLength = 500;
    
    public string Value { get; private set; }
    
    private TokenValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidRefreshTokenException(InvalidRefreshTokenException.TokenEmpty);
        if (value.Length > MaxLength)
            throw new InvalidRefreshTokenException(InvalidRefreshTokenException.TokenTooLong);
        Value = value;
    }
    
    public static TokenValue Create(string value) => new(value);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}