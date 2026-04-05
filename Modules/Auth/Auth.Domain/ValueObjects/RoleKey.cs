using System.Text.RegularExpressions;
using Auth.Domain.Common;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public sealed class RoleKey : ValueObject
{
    private const int MaxLength = 50;
    private static readonly Regex Pattern = new(@"^[a-z0-9_][a-z0-9_.:-]{1,49}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private RoleKey(string value)
    {
        Value = value;
    }

    public static RoleKey Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidRoleException(InvalidRoleException.CodeEmpty);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength || !Pattern.IsMatch(trimmed))
        {
            throw new InvalidRoleException(InvalidRoleException.CodeInvalid);
        }

        return new RoleKey(trimmed.ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

