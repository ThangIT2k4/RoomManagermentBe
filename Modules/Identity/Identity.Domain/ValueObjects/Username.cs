
using System.Text.RegularExpressions;
using Identity.Domain.Common;
using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

public sealed class Username : ValueObject
{
    // username ít nhất 2 ký tự trở lên và không chứa ký tự đặc biệt
    private static readonly Regex UsernameRegex = new Regex(
        @"^[a-zA-Z0-9]{2,}$"
    );
    
    public string Value { get; private set; }

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
        
        value = value.Trim();
        
        if (!UsernameRegex.IsMatch(value))
        {
            throw new InvalidUsernameException(InvalidUsernameException.CodeInvalid);
        }

        return new Username(value);
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public override string ToString() => Value;
}