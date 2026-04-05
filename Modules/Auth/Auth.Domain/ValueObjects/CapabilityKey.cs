using System.Text.RegularExpressions;
using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class CapabilityKey : ValueObject
{
    private const int MaxLength = 100;
    private static readonly Regex Pattern = new(@"^[a-z0-9_][a-z0-9_.:-]{1,99}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private CapabilityKey(string value)
    {
        Value = value;
    }

    public static CapabilityKey Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCapabilityException(InvalidCapabilityException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength || !Pattern.IsMatch(trimmed))
        {
            throw new InvalidCapabilityException(InvalidCapabilityException.CodeInvalid);
        }

        return new CapabilityKey(trimmed.ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

