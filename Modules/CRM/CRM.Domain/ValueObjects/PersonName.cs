using CRM.Domain.Common;
using CRM.Domain.Exceptions;

namespace CRM.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    public string Value { get; }

    private PersonName(string value)
    {
        Value = value;
    }

    public static PersonName Create(string value, string fieldName = "name")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} is required.");
        }

        var normalized = value.Trim();
        if (normalized.Length > InputSecurityLimits.MaxLeadNameLength)
        {
            throw new DomainValidationException($"{fieldName} length cannot exceed {InputSecurityLimits.MaxLeadNameLength}.");
        }

        return new PersonName(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
